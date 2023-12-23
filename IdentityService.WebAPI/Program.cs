using CommonHelper;
using Microsoft.EntityFrameworkCore;
using IdentityServiceInfrastructure;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Data.SqlClient;
using IdentityServiceDomain.Entity;
using IdentityServiceDomain;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServiceInfrastructure.Service;
using System.Reflection;

namespace IdentityService.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //数据库
            builder.Services.AddDbContext<IdDBContext>(ctx =>
            {
                //连接字符串如果放到appsettings.json中，会有泄密的风险
                //如果放到UserSecrets中，每个项目都要配置，很麻烦
                //因此这里推荐放到环境变量中。
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });

            //配置读取from数据库
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            builder.Configuration.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
            //日志信息输出
            builder.Services.AddLogging(builder =>
            {
                Log.Logger = new LoggerConfiguration()
                   .WriteTo.Console()
                   .WriteTo.File("IdentityService.log")
                   .CreateLogger();
                builder.AddSerilog();
            });
            Log.Logger.Information("hello");

            //开始基础设施层注入
            builder.Services.AddScoped<IdDomainService>();
            builder.Services.AddScoped<IIdRepository, IdRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ISmsSender, SmsSender>();

            //领域事件MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            # region Authentication,Authorization
            //只要需要校验Authentication报文头的地方（非IdentityService.WebAPI项目）也需要启用这些
            //IdentityService项目还需要启用AddIdentityCore
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();
            JWTOptions jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
            builder.Services.AddJWTAuthentication(jwtOpt);
            //启用Swagger中的【Authorize】按钮。
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });
            #endregion

            builder.Services.AddDataProtection();
            //登录、注册的项目除了要启用WebApplicationBuilderExtensions中的初始化之外，还要如下的初始化
            //不要用AddIdentity，而是用AddIdentityCore
            //因为用AddIdentity会导致JWT机制不起作用，AddJwtBearer中回调不会被执行，因此总是Authentication校验失败
            //https://github.com/aspnet/Identity/issues/1376
            IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(options =>
            {
                //三次登录失败锁定用户5分钟
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.User.AllowedUserNameCharacters = null;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                //不能设定RequireUniqueEmail，否则不允许邮箱为空
                //options.User.RequireUniqueEmail = true;

                //以下两行，把GenerateEmailConfirmationTokenAsync验证码缩短
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            }
            );
            idBuilder = new IdentityBuilder(idBuilder.UserType, typeof(IdentityServiceDomain.Entity.Role), builder.Services);
            idBuilder.AddEntityFrameworkStores<IdDBContext>().AddDefaultTokenProviders()
                //.AddRoleValidator<RoleValidator<Role>>()
                .AddRoleManager<RoleManager<IdentityServiceDomain.Entity.Role>>()
                .AddUserManager<IdUserManager>();

            // Configure the HTTP request pipeline.


            //Redis的配置
            string redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnStr");
            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
            builder.Services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);


            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseForwardedHeaders();


            app.MapControllers();

            app.Run();
        }
    }
}
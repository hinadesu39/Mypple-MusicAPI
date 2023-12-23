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


            //���ݿ�
            builder.Services.AddDbContext<IdDBContext>(ctx =>
            {
                //�����ַ�������ŵ�appsettings.json�У�����й�ܵķ���
                //����ŵ�UserSecrets�У�ÿ����Ŀ��Ҫ���ã����鷳
                //��������Ƽ��ŵ����������С�
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });

            //���ö�ȡfrom���ݿ�
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            builder.Configuration.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
            //��־��Ϣ���
            builder.Services.AddLogging(builder =>
            {
                Log.Logger = new LoggerConfiguration()
                   .WriteTo.Console()
                   .WriteTo.File("IdentityService.log")
                   .CreateLogger();
                builder.AddSerilog();
            });
            Log.Logger.Information("hello");

            //��ʼ������ʩ��ע��
            builder.Services.AddScoped<IdDomainService>();
            builder.Services.AddScoped<IIdRepository, IdRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ISmsSender, SmsSender>();

            //�����¼�MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            # region Authentication,Authorization
            //ֻҪ��ҪУ��Authentication����ͷ�ĵط�����IdentityService.WebAPI��Ŀ��Ҳ��Ҫ������Щ
            //IdentityService��Ŀ����Ҫ����AddIdentityCore
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();
            JWTOptions jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
            builder.Services.AddJWTAuthentication(jwtOpt);
            //����Swagger�еġ�Authorize����ť��
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });
            #endregion

            builder.Services.AddDataProtection();
            //��¼��ע�����Ŀ����Ҫ����WebApplicationBuilderExtensions�еĳ�ʼ��֮�⣬��Ҫ���µĳ�ʼ��
            //��Ҫ��AddIdentity��������AddIdentityCore
            //��Ϊ��AddIdentity�ᵼ��JWT���Ʋ������ã�AddJwtBearer�лص����ᱻִ�У��������AuthenticationУ��ʧ��
            //https://github.com/aspnet/Identity/issues/1376
            IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(options =>
            {
                //���ε�¼ʧ�������û�5����
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                options.User.AllowedUserNameCharacters = null;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                //�����趨RequireUniqueEmail��������������Ϊ��
                //options.User.RequireUniqueEmail = true;

                //�������У���GenerateEmailConfirmationTokenAsync��֤������
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


            //Redis������
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
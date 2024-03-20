using CommonHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicAdmin.WebAPI;
using MusicDomain;
using MusicInfrastructure;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MusicMain.WebAPI
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

            //缓存
            builder.Services.AddMemoryCache();

            //数据库
            builder.Services.AddDbContext<MusicDBContext>(ctx =>
            {

                //连接字符串放到环境变量中安全。
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });
            // 配置读取from数据库
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            builder.Configuration.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));

            //redis
            string redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnStr");
            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
            builder.Services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);

            //工作单元注入
            builder.Services.Configure<MvcOptions>(o =>
            {
                //注册全局的filter
                o.Filters.Add<UnitOfWorkFilter>();
            });

            #region Authentication,Authorization
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

            //基础设施层注入
            builder.Services.AddScoped<MusicDomainService>();
            builder.Services.AddScoped<IMusicRepository, MusicRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseForwardedHeaders();


            app.MapControllers();

            app.Run();
        }
    }
}
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

            //����
            builder.Services.AddMemoryCache();

            //���ݿ�
            builder.Services.AddDbContext<MusicDBContext>(ctx =>
            {

                //�����ַ����ŵ����������а�ȫ��
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });
            // ���ö�ȡfrom���ݿ�
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            builder.Configuration.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));

            //redis
            string redisConnStr = builder.Configuration.GetValue<string>("Redis:ConnStr");
            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
            builder.Services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);

            //������Ԫע��
            builder.Services.Configure<MvcOptions>(o =>
            {
                //ע��ȫ�ֵ�filter
                o.Filters.Add<UnitOfWorkFilter>();
            });

            #region Authentication,Authorization
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

            //������ʩ��ע��
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
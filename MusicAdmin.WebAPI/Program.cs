using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicDomain;
using MusicInfrastructure;
using StackExchange.Redis;

namespace MusicAdmin.WebAPI
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

            //������ʩ��ע��
            builder.Services.AddScoped<MusicDomainService>();
            builder.Services.AddScoped<IMusicRepository, MusicRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
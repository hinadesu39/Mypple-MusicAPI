using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicDomain;
using MusicInfrastructure;

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

            //数据库
            builder.Services.AddDbContext<MusicDBContext>(ctx =>
            {
                
                //连接字符串放到环境变量中安全。
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });

            //工作单元注入
            builder.Services.Configure<MvcOptions>(o =>
            {
                //注册全局的filter
                o.Filters.Add<UnitOfWorkFilter>();
            });

            //基础设施层注入
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
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

            //���ݿ�
            builder.Services.AddDbContext<MusicDBContext>(ctx =>
            {
                
                //�����ַ����ŵ����������а�ȫ��
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });

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
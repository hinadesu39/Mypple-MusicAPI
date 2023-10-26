using FileServiceDomain;
using FileServiceInfrastructure;
using FileServiceInfrastructure.StorageClient;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FileService.WebAPI
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

            //���ö�ȡfrom���ݿ�
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            builder.Configuration.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
            builder.Services//.AddOptions() //asp.net core��Ŀ��AddOptions()��дҲ�У���Ϊ���һ���Զ�ִ����
                .Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"));
            //����

            builder.Services.Configure<MvcOptions>(o =>
            {
                //ע��ȫ�ֵ�filter
                o.Filters.Add<UnitOfWorkFilter>();
            });

            ////�ļ���С����
            //builder.Services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestBodySize = int.MaxValue;
            //});

            //��ʼ������ʩ��ע��
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IStorageClient, SMBStorageClient>();
            //���浽�ƴ洢�У�����Ŀ��ʱ����
            //ʹ��������ƴ洢
            builder.Services.AddScoped<IStorageClient, MockCloudStorageClient>();
            builder.Services.AddScoped<IFSRepository, FSRepository>();
            builder.Services.AddScoped<FileDomainService>();
            builder.Services.AddHttpClient();
            //����������ʩ��ע��

            //���ݿ�
            builder.Services.AddDbContext<FileServiceDBContext>(ctx =>
            {
                //�����ַ�������ŵ�appsettings.json�У�����й�ܵķ���
                //����ŵ�UserSecrets�У�ÿ����Ŀ��Ҫ���ã����鷳
                //��������Ƽ��ŵ����������С�
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });

            //ת������
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

            app.UseAuthorization();
            app.UseForwardedHeaders();
            app.MapControllers();

            // Set up custom content types - associating file extension to MIME type
            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".mp3"] = "audio/mp3";
            provider.Mappings[".flac"] = "audio/flac";
            provider.Mappings[".png"] = "image/png";
            provider.Mappings[".jpg"] = "image/jpg";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
            app.Run();
        }
    }
}
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

            //配置读取from数据库
            string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
            builder.Configuration.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
            builder.Services//.AddOptions() //asp.net core项目中AddOptions()不写也行，因为框架一定自动执行了
                .Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"));
            //结束

            builder.Services.Configure<MvcOptions>(o =>
            {
                //注册全局的filter
                o.Filters.Add<UnitOfWorkFilter>();
            });

            ////文件大小限制
            //builder.Services.Configure<IISServerOptions>(options =>
            //{
            //    options.MaxRequestBodySize = int.MaxValue;
            //});

            //开始基础设施层注入
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IStorageClient, SMBStorageClient>();
            //保存到云存储中，本项目暂时不用
            //使用虚拟的云存储
            builder.Services.AddScoped<IStorageClient, MockCloudStorageClient>();
            builder.Services.AddScoped<IFSRepository, FSRepository>();
            builder.Services.AddScoped<FileDomainService>();
            builder.Services.AddHttpClient();
            //结束基础设施层注入

            //数据库
            builder.Services.AddDbContext<FileServiceDBContext>(ctx =>
            {
                //连接字符串如果放到appsettings.json中，会有泄密的风险
                //如果放到UserSecrets中，每个项目都要配置，很麻烦
                //因此这里推荐放到环境变量中。
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                ctx.UseSqlServer(connStr);
            });

            //转发配置
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
using FileServiceDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace FileServiceInfrastructure.StorageClient
{
    /// <summary>
    /// 模拟云存储的使用，文件保存在wwwroot文件夹下。
    /// </summary>
    public class MockCloudStorageClient : IStorageClient
    {
        public StorageType StorageType => StorageType.Public;
        private readonly IWebHostEnvironment hostEnv;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MockCloudStorageClient(IWebHostEnvironment hostEnv, IHttpContextAccessor httpContextAccessor)
        {
            this.hostEnv = hostEnv;
            this.httpContextAccessor = httpContextAccessor;
        }

        //IWebHostEnvironment 接口提供了有关 Web 主机环境的信息，例如应用程序的内容根路径和 Web 根路径,由 ASP.NET Core 自动注册。
        //IHttpContextAccessor 接口提供了对当前请求上下文的访问。你可以使用它来获取当前请求的 HttpContext对象,在ModuleInitializer中注册。
        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith("/"))
            {
                throw new ArgumentException("key should not start with /", nameof(key));

            }
            string workingDir = Path.Combine(hostEnv.ContentRootPath, "wwwroot");
            string fullPath = Path.Combine(workingDir, key);
            string? fullDir = Path.GetDirectoryName(fullPath);//get the directory
            if (!Directory.Exists(fullDir))//automatically create dir
            {
                Directory.CreateDirectory(fullDir);
            }
            if (File.Exists(fullPath))//如果已经存在，则尝试删除
            {
                File.Delete(fullPath);
            }
            using Stream outStream = File.OpenWrite(fullPath);
            await content.CopyToAsync(outStream, cancellationToken);
            var req = httpContextAccessor.HttpContext.Request;
            string url = req.Scheme + "://" + req.Host + "/FileService/" + key;
            return new Uri(url);
        }
    }
}

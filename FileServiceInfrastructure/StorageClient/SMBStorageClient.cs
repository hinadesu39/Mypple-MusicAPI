using FileServiceDomain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServiceInfrastructure.StorageClient
{
    public class SMBStorageClient : IStorageClient
    {
        public StorageType StorageType => StorageType.Backup;
        private readonly SMBStorageOptions _options;

        //保证配置更改也能实时更新
        public SMBStorageClient(IOptionsSnapshot<SMBStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default)
        {
            if (key.StartsWith("/"))
            {
                throw new ArgumentException("key should not start with /", nameof(key));

            }
            string workingDir = _options.WorkingDir;
            //给文件安排一个位置
            string fullPath = Path.Combine(workingDir, key);
            string? fullDir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
            }
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            //写入
            using Stream outstream = File.OpenWrite(fullPath);
            await content.CopyToAsync(outstream, cancellationToken);
            //将 content 流中的数据异步复制到 outStream 流中
            //当这两行代码执行完成后，content 流中的所有数据都将被复制到指定路径的文件中。
            return new Uri(fullPath);
        }
    }
}

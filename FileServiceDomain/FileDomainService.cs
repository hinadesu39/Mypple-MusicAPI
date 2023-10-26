using CommonHelper;
using FileServiceDomain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServiceDomain
{
    public class FileDomainService
    {
        private readonly IFSRepository repository;
        private readonly IStorageClient backupStorage;//备份服务器
        private readonly IStorageClient remoteStorage;//文件存储服务器

        public FileDomainService(IFSRepository repository,
            IEnumerable<IStorageClient> storageClients)
        {
            this.repository = repository;
            //用这种方式可以解决内置DI不能使用名字注入不同实例的问题，而且从原则上来讲更加优美
            this.backupStorage = storageClients.First(c => c.StorageType == StorageType.Backup);
            this.remoteStorage = storageClients.First(c => c.StorageType == StorageType.Public);
        }

        public async Task<UploadedItem> UploadAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            string hash = HashHelper.ComputeSha256Hash(stream);
            long fileSize = stream.Length;
            DateTime today = DateTime.Today;
            //用日期把文件分散在不同文件夹存储，同时由于加上了文件hash值作为目录，又用用户上传的文件夹做文件名，
            //所以几乎不会发生不同文件冲突的可能
            //用用户上传的文件名保存文件名，这样用户查看、下载文件的时候，文件名更灵活
            string key = $"{today.Year}/{today.Month}/{today.Day}/{fileName}";

            stream.Position = 0;
            Uri backUpUrl = await backupStorage.SaveAsync(key, stream, cancellationToken);
            stream.Position = 0;
            Uri remoteUrl = await remoteStorage.SaveAsync(key, stream, cancellationToken);
            
            //领域服务并不会真正的执行数据库插入，只是把实体对象生成，然后由应用服务和基础设施配合来真正的插入数据库！
            //DDD中尽量避免直接在领域服务中执行数据库的修改（包含删除、新增）操作。
            Guid guid = Guid.NewGuid();
            return UploadedItem.Create(guid, fileSize, fileName, hash, backUpUrl, remoteUrl);
        }
    }
}

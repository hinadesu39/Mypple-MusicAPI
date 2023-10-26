using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServiceDomain
{
    public interface IStorageClient
    {
        StorageType StorageType { get; }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="key">文件的key（一般是文件路径的一部分）</param>
        /// <param name="content">文件内容</param>
        /// <param name="cancellationToken">在这个方法中，最后一个参数 CancellationToken cancellationToken = default 是一个可选参数，它用于取消异步操作。如果你不想取消操作，可以不传递这个参数，它会使用默认值
        /// <returns>存储返回的可以被访问的文件Url</returns>

        Task<Uri> SaveAsync(string key, Stream content, CancellationToken cancellationToken = default);
    }
}

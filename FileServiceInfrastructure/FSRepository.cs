using FileServiceDomain;
using FileServiceDomain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServiceInfrastructure
{
    public class FSRepository : IFSRepository
    {
        private readonly FileServiceDBContext _dbContext;

        public FSRepository(FileServiceDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash)
        {
            return _dbContext.UploadItems.FirstOrDefaultAsync(u => u.FileSizeInBytes == fileSize
            && u.FileSHA256Hash == sha256Hash);
        }

    }
}

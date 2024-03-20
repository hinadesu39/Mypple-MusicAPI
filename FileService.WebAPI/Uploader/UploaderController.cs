using CommonHelper;
using FileServiceDomain;
using FileServiceInfrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FileService.WebAPI.Uploader
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploaderController : ControllerBase
    {
        private readonly FileServiceDBContext fSDBContext;
        private readonly FileDomainService fSDomainService;
        private readonly IFSRepository fSRepository;
        private readonly ILogger<UploaderController> logger;

        public UploaderController(
            FileServiceDBContext fSDBContext,
            FileDomainService fSDomainService,
            IFSRepository fSRepository,
            ILogger<UploaderController> logger
        )
        {
            this.fSDBContext = fSDBContext;
            this.fSDomainService = fSDomainService;
            this.fSRepository = fSRepository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<FileExistsResponse> FileExists(long fileSize, string sha256Hash)
        {
            logger.LogWarning($"文件长度{fileSize},哈希值{sha256Hash}");
            var item = await fSRepository.FindFileAsync(fileSize, sha256Hash);
            if (item == null)
            {
                return new FileExistsResponse(false, null);
            }
            else
            {
                return new FileExistsResponse(true, item.RemoteUrl);
            }
        }

        [UnitOfWork(typeof(FileServiceDBContext))]
        [RequestSizeLimit(160_000_000)]
        [HttpPost]
        public async Task<ActionResult<Uri>> Upload(
            [FromForm] UploadRequest request,
            CancellationToken cancellationToken
        )
        {
            var file = request.File;
            string fileName = file.FileName;
            using Stream stream = file.OpenReadStream();
            var (upItem, res) = await fSDomainService.UploadAsync(stream, fileName, cancellationToken);
            if (!res)
            {
                fSDBContext.Add(upItem);
            }
            return upItem.RemoteUrl;
        }
    }
}

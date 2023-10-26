using FileServiceDomain;
using FileServiceInfrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileService.WebAPI.Uploader
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploaderController : ControllerBase
    {
        private readonly FileServiceDBContext fSDBContext;
        private readonly FileDomainService fSDomainService;
        private readonly IFSRepository fSRepository;

        public UploaderController(
            FileServiceDBContext fSDBContext,
            FileDomainService fSDomainService,
            IFSRepository fSRepository
        )
        {
            this.fSDBContext = fSDBContext;
            this.fSDomainService = fSDomainService;
            this.fSRepository = fSRepository;
        }

        [HttpGet]
        public async Task<FileExistsResponse> FileExists(long fileSize, string sha256Hash)
        {
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

            var itme = await fSDBContext.UploadItems.FirstOrDefaultAsync(
                x => x.FileName == fileName
            );
            if (itme != null)
            {
                return itme.RemoteUrl;
            }
            else
            {
                using Stream stream = file.OpenReadStream();
                var upItem = await fSDomainService.UploadAsync(stream, fileName, cancellationToken);
                fSDBContext.Add(upItem);
                return upItem.RemoteUrl;
            }
        }
    }
}

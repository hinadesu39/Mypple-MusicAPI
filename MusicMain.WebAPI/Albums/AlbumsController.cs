using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI;
using MusicDomain.Entity;
using MusicDomain;
using MusicInfrastructure;

namespace MusicMain.WebAPI.Albums
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    public class AlbumsController : ControllerBase
    {
        private readonly MusicDBContext musicDBContext;
        private readonly MusicDomainService musicDomainService;
        private readonly IMusicRepository musicRepository;

        public AlbumsController(
            MusicDBContext musicDBContext,
            MusicDomainService musicDomainService,
            IMusicRepository musicRepository
        )
        {
            this.musicDBContext = musicDBContext;
            this.musicDomainService = musicDomainService;
            this.musicRepository = musicRepository;
        }
        [HttpGet]
        public async Task<ActionResult<Album[]>> GetAll()
        {
            return await musicRepository.GetAlbumsAsync();
        }

        [HttpGet]
        public async Task<ActionResult<Album?>> GetById(Guid albumId)
        {
            return await musicRepository.GetAlbumByIdAsync(albumId);
        }

        [HttpGet]
        public async Task<ActionResult<Album[]>> GetByName(string name)
        {
            return await musicRepository.GetAlbumByNameAsync(name);
        }

        [HttpGet]
        public async Task<ActionResult<Album[]>> GetByArtistId(Guid artistId)
        {
            return await musicRepository.GetAlbumsByArtistIdAsync(artistId);
        }
    }
}

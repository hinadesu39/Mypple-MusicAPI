using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI;
using MusicDomain.Entity;
using MusicDomain;
using MusicInfrastructure;

namespace MusicMain.WebAPI.Musics
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    public class MusicsController : ControllerBase
    {
        private readonly MusicDBContext musicDBContext;
        private readonly MusicDomainService musicDomainService;
        private readonly IMusicRepository musicRepository;

        public MusicsController(
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
        public async Task<ActionResult<Music[]>> GetAll()
        {
            return await musicRepository.GetMusicsAsync();
        }
        [HttpGet]
        public async Task<ActionResult<Music?>> GetById(Guid id)
        {
            return await musicRepository.GetMusicByIdAsync(id);
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByName(string name)
        {
            return await musicRepository.GetMusicsByNameAsync(name);
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByAlbumId(Guid albumId)
        {
            return await musicRepository.GetMusicsByAlbumIdAsync(albumId);
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByArtistId(Guid artistId)
        {
            return await musicRepository.GetMusicsByArtistIdAsync(artistId);
        }
    }
}

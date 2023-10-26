using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicDomain;
using MusicDomain.Entity;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Artists
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    public class ArtistsController : ControllerBase
    {
        private readonly MusicDBContext musicDBContext;
        private readonly MusicDomainService musicDomainService;
        private readonly IMusicRepository musicRepository;

        public ArtistsController(
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
        public async Task<ActionResult<Artist[]>> GetAll()
        {
            return await musicRepository.GetArtistsAsync();
        }

        [HttpGet]
        public async Task<ActionResult<Artist?>> GetArtistById(Guid artistId)
        {
            return await musicRepository.GetArtistByIdAsync(artistId);
        }

        [HttpGet]
        public async Task<ActionResult<Artist[]>> GetArtistByName(string name)
        {
            return await musicRepository.GetArtistByNameAsync(name);
        }
    }
}

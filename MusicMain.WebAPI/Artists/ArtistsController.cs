using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI;
using MusicDomain.Entity;
using MusicDomain;
using MusicInfrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace MusicMain.WebAPI.Artists
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    public class ArtistsController : ControllerBase
    {
        private readonly MusicDBContext musicDBContext;
        private readonly MusicDomainService musicDomainService;
        private readonly IMusicRepository musicRepository;
        private readonly IMemoryCache memoryCache;

        public ArtistsController(
            MusicDBContext musicDBContext,
            MusicDomainService musicDomainService,
            IMusicRepository musicRepository,
            IMemoryCache memoryCache
        )
        {
            this.musicDBContext = musicDBContext;
            this.musicDomainService = musicDomainService;
            this.musicRepository = musicRepository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<Artist[]>> GetAll()
        {
            var Artist = await memoryCache.GetOrCreateAsync(
                $"ArtistsController.GetAll",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetArtistsAsync();
                }
            );
            if (Artist == null)
                return NotFound();
            return Artist;
        }

        [HttpGet]
        public async Task<ActionResult<Artist?>> GetById(Guid artistId)
        {
            var Artist = await memoryCache.GetOrCreateAsync(
                $"ArtistsController.GetById.{artistId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetArtistByIdAsync(artistId);
                }
            );
            if (Artist == null)
                return NotFound();
            return Artist;
        }

        [HttpGet]
        public async Task<ActionResult<Artist[]>> GetByName(string name)
        {
            var Artist = await memoryCache.GetOrCreateAsync(
                $"ArtistsController.GetByName.{name}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetArtistByNameAsync(name);
                }
            );
            if (Artist == null)
                return NotFound();
            return Artist;
        }
    }
}

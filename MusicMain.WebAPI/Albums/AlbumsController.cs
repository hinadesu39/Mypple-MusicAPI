using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI;
using MusicDomain.Entity;
using MusicDomain;
using MusicInfrastructure;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;

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
        private readonly IMemoryCache memoryCache;

        public AlbumsController(
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
        public async Task<ActionResult<Album[]>> GetAll()
        {

            var Album = await memoryCache.GetOrCreateAsync(
                $"AlbumsController.GetAll",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetAlbumsAsync();
                }
            );
            if (Album == null)
                return NotFound();
            return Album;
        }

        [HttpGet]
        public async Task<ActionResult<Album?>> GetById(Guid albumId)
        {
            var Album = await memoryCache.GetOrCreateAsync(
                $"AlbumsController.GetById.{albumId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetAlbumByIdAsync(albumId);
                }
            );
            if (Album == null)
                return NotFound();
            return Album;
        }

        [HttpGet]
        public async Task<ActionResult<Album[]>> GetByName(string name)
        {
            var Album = await memoryCache.GetOrCreateAsync(
                $"AlbumsController.GetByName.{name}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetAlbumByNameAsync(name);
                }
            );
            if (Album == null)
                return NotFound();
            return Album;
        }

        [HttpGet]
        public async Task<ActionResult<Album[]>> GetByArtistId(Guid artistId)
        {
            var Album = await memoryCache.GetOrCreateAsync(
                $"AlbumsController.GetByArtistId.{artistId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetAlbumsByArtistIdAsync(artistId);
                }
            );
            if (Album == null)
                return NotFound();
            return Album;
        }

        [HttpGet]
        public async Task<ActionResult<Album[]>> GetAlbumsByMusicPostOrder()
        {
            var Album = await memoryCache.GetOrCreateAsync(
                $"AlbumsController.GetAlbumsByMusicPostOrder",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetAlbumsByMusicPostOrder();
                }
            );
            if (Album == null)
                return NotFound();
            return Album;
        }
    }
}

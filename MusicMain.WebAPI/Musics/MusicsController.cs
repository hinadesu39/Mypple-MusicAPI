using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI;
using MusicDomain.Entity;
using MusicDomain;
using MusicInfrastructure;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;
using MusicDomain.Entity.DTO;

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
        private readonly IMemoryCache memoryCache;

        public MusicsController(
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
        public async Task<ActionResult<Music[]>> GetAll()
        {
            var Music = await memoryCache.GetOrCreateAsync(
                $"MusicsController.GetAll",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetMusicsAsync();
                }
            );
            if (Music == null)
                return NotFound();
            return Music;
        }

        [HttpGet]
        public async Task<ActionResult<Music?>> GetById(Guid id)
        {
            var Music = await memoryCache.GetOrCreateAsync(
                $"MusicsController.GetById.{id}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetMusicByIdAsync(id);
                }
            );
            if (Music == null)
                return NotFound();
            return Music;
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByName(string name)
        {
            var Music = await memoryCache.GetOrCreateAsync(
                $"MusicsController.GetByName.{name}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetMusicsByNameAsync(name);
                }
            );
            if (Music == null)
                return NotFound();
            return Music;
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByAlbumId(Guid albumId)
        {
            var Music = await memoryCache.GetOrCreateAsync(
                $"MusicsController.GetByAlbumId.{albumId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetMusicsByAlbumIdAsync(albumId);
                }
            );
            if (Music == null)
                return NotFound();
            return Music;
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByArtistId(Guid artistId)
        {
            var Music = await memoryCache.GetOrCreateAsync(
                $"MusicsController.GetByArtistId.{artistId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetMusicsByArtistIdAsync(artistId);
                }
            );
            if (Music == null)
                return NotFound();
            return Music;
        }

        [HttpGet]
        public async Task<ActionResult<MusicDTO[]>> GetByPlayListId(Guid playListId)
        {
            var Music = await memoryCache.GetOrCreateAsync(
                $"MusicsController.GetByPlayListId.{playListId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetMusicsByPlayListIdAsync(playListId);
                }
            );
            if (Music == null)
                return NotFound();
            return Music;
        }
    }
}

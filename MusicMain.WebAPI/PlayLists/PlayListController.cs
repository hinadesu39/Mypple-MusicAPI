using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MusicAdmin.WebAPI;
using MusicDomain;
using MusicDomain.Entity;
using MusicDomain.Entity.DTO;
using MusicInfrastructure;
using MusicMain.WebAPI.PlayLists.Request;

namespace MusicMain.WebAPI.PlayLists
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    public class PlayListController : ControllerBase
    {
        private readonly IMusicRepository musicRepository;
        private readonly IMemoryCache memoryCache;

        public PlayListController(IMusicRepository musicRepository, IMemoryCache memoryCache)
        {
            this.musicRepository = musicRepository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<PlayListDTO[]>> GetAll()
        {
            var playList = await memoryCache.GetOrCreateAsync(
                $"PlayListController.GetAll",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetPlayListAsync();
                }
            );
            if (playList == null)
                return NotFound();
            return playList;
        }

        [HttpGet]
        public async Task<ActionResult<PlayListDTO>> GetById(Guid id)
        {
            var playList = await memoryCache.GetOrCreateAsync(
              $"PlayListController.GetById",
              async (e) =>
              {
                  e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                  return await musicRepository.GetPlayListByIdAsync(id);
              }
          );
            if (playList == null)
                return NotFound();
            return playList;
        }

        [HttpPost]
        public async Task<ActionResult<PlayList>> Add(PlayListAddRequest request)
        {
            return await musicRepository.AddPlayListAsync(
                request.PicUrl,
                request.Title,
                request.Description
            );
        }

        [HttpPost]
        public async Task<ActionResult<Music[]>> AddMusicToPlayList(
            [FromBody] MusicAddToPlayListRequest request
        )
        {
            var musicList = request.musics
                .Select(
                    m =>
                        Music.Create(
                            m.AudioUrl,
                            m.PicUrl,
                            m.Title,
                            m.Duration,
                            m.Artist,
                            m.ArtistId,
                            m.Album,
                            m.AlbumId,
                            m.Type,
                            m.Lyric,
                            m.PublishTime
                        )
                )
                .ToArray();
            return await musicRepository.AddMusicToPlayListAsync(request.PlayListId, musicList);
        }   
    }
}

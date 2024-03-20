using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MusicAdmin.WebAPI;
using MusicDomain;
using MusicDomain.Entity;
using MusicDomain.Entity.DTO;
using MusicInfrastructure;
using MusicMain.WebAPI.PlayLists.Request;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PlayListDTO[]>> GetAll()
        {
            Guid userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var playList = await memoryCache.GetOrCreateAsync(
                $"PlayListController.GetAll.{userId}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetPlayListAsync(userId);
                }
            );
            if (playList == null)
                return NotFound();
            return playList;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PlayListDTO>> GetById(Guid id)
        {
            Guid userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var playList = await memoryCache.GetOrCreateAsync(
                $"PlayListController.GetById.{id}",
                async (e) =>
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(Random.Shared.Next(30, 45)));
                    return await musicRepository.GetPlayListByIdAsync(userId, id);
                }
            );
            if (playList == null)
                return NotFound();
            return playList;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PlayList>> Add(PlayListAddRequest request)
        {
            Guid userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            //删除旧缓存
            memoryCache.Remove($"PlayListController.GetAll.{userId}");
            return await musicRepository.AddPlayListAsync(
                userId,
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
            //删除旧缓存
            memoryCache.Remove($"MusicsController.GetByPlayListId.{request.PlayListId}");
            return await musicRepository.AddMusicToPlayListAsync(request.PlayListId, musicList);
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

        [HttpPost]
        public async Task<ActionResult<bool>> RemoveMusicFromPlayList(RemoveMusicFromPlayListRequest req)
        {
            return await musicRepository.RemoveMusicFromPlayListAsync(req.PlayListId, req.MusicId);
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            //删除旧缓存
            Guid userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            memoryCache.Remove($"PlayListController.GetAll.{userId}");
            memoryCache.Remove($"PlayListController.GetById.{id}");
            return await musicRepository.RemovePlayListAsync(userId, id);
        }
    }
}

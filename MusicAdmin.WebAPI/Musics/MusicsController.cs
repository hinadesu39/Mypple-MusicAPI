using CommonHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAdmin.WebAPI.Musics.Request;
using MusicDomain;
using MusicDomain.Entity;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Musics
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

        [HttpPost]
        public async Task<ActionResult<Guid>> Add(MusicAddRequest req)
        {
            //如果该歌手不存在则创建
            var artist = await musicRepository.GetArtistByNameAsync(req.Artist);
            Artist newArtist;
            if (artist.Length != 0)
                newArtist = artist[0];
            else
            {
                newArtist = await musicDomainService.AddArtist(req.ArtistPicUrl, req.Artist);
                await musicDBContext.AddAsync(newArtist);
            }

            //如果该专辑不存在则创建
            var album = await musicRepository.GetAlbumByNameAsync(req.Album);
            Album newAlbum;
            if (album.Length != 0)
                newAlbum = album[0];
            else
            {
                newAlbum = await musicDomainService.AddAlbum(req.AlbumPicUrl, req.Album, req.Artist, newArtist.Id, req.Type, req.PublishTime);
                await musicDBContext.AddAsync(newAlbum);
            }

            //如果该歌曲不存在则创建，否则直接返回
            var music = await musicRepository.GetMusicsByNameAsync(req.Title);
            if (music.Length != 0)
                return music[0].Id;
            Music newMusic = await musicDomainService.AddMusic(
                req.AudioUrl,
                req.MusicPicUrl,
                req.Title,
                req.Duration,
                req.Artist,
                newArtist.Id,
                req.Album,
                newAlbum.Id,
                req.Type,
                req.Lyric,
                req.PublishTime
            );
            await musicDBContext.AddAsync(newMusic);
            return newMusic.Id;
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetAll()
        {
            return await musicRepository.GetMusicsAsync();
        }

        [HttpGet]
        public async Task<ActionResult<Music[]>> GetByName(string keyWords)
        {
            return await musicRepository.GetMusicsByNameAsync(keyWords);
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

        [HttpDelete]
        public async Task<ActionResult> DeleteById(Guid musicId)
        {
            var music = await musicRepository.GetMusicByIdAsync(musicId);
            if(music == null)
            {
                return NotFound($"没有Id={musicId}的Music");
            }
            //软删除
            music.SoftDelete();
            return Ok(music);
        }
    }
}

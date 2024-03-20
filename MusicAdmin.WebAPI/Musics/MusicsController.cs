using CommonHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicAdmin.WebAPI.Albums.Request;
using MusicAdmin.WebAPI.Musics.Request;
using MusicDomain;
using MusicDomain.Entity;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Musics
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult<string>> Add(MusicAddRequest req)
        {
            //如果该歌手不存在则创建
            var artist = await musicRepository.GetArtistByNameAsync(req.Artist);
            Artist newArtist;
            if (artist.Length != 0 && artist.FirstOrDefault(a => a.Name == req.Artist) != null)
                newArtist = artist.FirstOrDefault(a => a.Name == req.Artist);
            else
            {
                newArtist = await musicDomainService.AddArtist(req.MusicPicUrl, req.Artist);
                await musicDBContext.AddAsync(newArtist);
            }

            //如果该专辑不存在则创建
            var album = await musicRepository.GetAlbumByNameAsync(req.Album);
            Album newAlbum;
            if (album.Length != 0 && album.FirstOrDefault(a => a.Title == req.Album) != null)
                newAlbum = album.FirstOrDefault(a => a.Title == req.Album);
            else
            {
                newAlbum = await musicDomainService.AddAlbum(
                    req.MusicPicUrl,
                    req.Album,
                    req.Artist,
                    newArtist.Id,
                    req.Type,
                    req.PublishTime
                );
                await musicDBContext.AddAsync(newAlbum);
            }

            //如果该歌曲不存在则创建，否则直接返回
            var music = await musicRepository.MusicExist(
                req.Title,
                req.Duration,
                req.Artist,
                req.Album
            );
            if (music != null)
                return music.Title;
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
            return newMusic.Title;
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

        [HttpPut]
        public async Task<ActionResult<string>> Update(MusicUpdateRequest req)
        {
            var music = await musicRepository.GetMusicByIdAsync(req.Id);
            if (music == null)
            {
                return NotFound($"没有Id={req.Id}的Music");
            }
            else
            {
                music.Update(
                    req.AudioUrl,
                    req.MusicPicUrl,
                    req.Title,
                    req.Artist,
                    req.ArtistId,
                    req.Album,
                    req.AlbumId,
                    req.Type,
                    req.Lyric,
                    req.PublishTime
                );
                return Ok("OK");
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteById(Guid id)
        {
            var music = await musicRepository.GetMusicByIdAsync(id);
            if (music == null)
            {
                return NotFound($"没有Id={id}的Music");
            }
            //软删除
            music.SoftDelete();
            return Ok("Ok");
        }
    }
}

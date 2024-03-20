using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI.Albums.Request;
using MusicAdmin.WebAPI.Artists.Request;
using MusicDomain;
using MusicDomain.Entity;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Albums
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    [Authorize(Roles = "Admin")]
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

        [HttpPost]
        public async Task<ActionResult<string>> Add(AlbumAddRequest req)
        {
            var res = await musicRepository.AlbumExist(req.Title,req.Artist,req.PublishTime);
            if (res == null)
            {
                var album = await musicDomainService.AddAlbum(req.PicUrl,req.Title,req.Artist,req.ArtistId,req.Type,req.PublishTime);
                await musicDBContext.AddAsync(album);
                return Ok(album.Title);
            }
            else
            {
                return BadRequest($"{req.Title} Existed");
            }
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

        [HttpPut]
        public async Task<ActionResult<string>> Update(AlbumUpdateRequest req)
        {
            var album = await musicRepository.GetAlbumByIdAsync(req.Id);
            if (album == null)
            {
                return NotFound($"没有Id={req.Id}的Album");
            }
            else
            {
                album.Update(req.PicUrl,req.Title, req.ArtistId, req.Artist, req.Type, req.PublishTime);
                return Ok("OK");
            }
        }


        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteById(Guid id)
        {
            var artist = await musicRepository.GetAlbumByIdAsync(id);
            if (artist == null)
            {
                return NotFound($"没有Id={id}的Album");
            }
            //软删除
            artist.SoftDelete();
            return Ok("Ok");
        }
    }
}

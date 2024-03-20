using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI.Artists.Request;
using MusicAdmin.WebAPI.Musics.Request;
using MusicDomain;
using MusicDomain.Entity;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Artists
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    [Authorize(Roles = "Admin")]
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

        [HttpPost]
        public async Task<ActionResult<string>> Add(ArtistAddRequest req)
        {
            var res = await musicRepository.ArtistExist(req.Name);
            if (res == null)
            {
                var artist = await musicDomainService.AddArtist(req.PicUrl, req.Name);
                await musicDBContext.AddAsync(artist);
                return Ok(artist.Name);
            }
            else
            {
                return BadRequest($"{req.Name} Existed");
            }
        }


        [HttpGet]
        public async Task<ActionResult<Artist[]>> GetAll()
        {
            return await musicRepository.GetArtistsAsync();
        }

        [HttpGet]
        public async Task<ActionResult<Artist?>> GetById(Guid artistId)
        {
            return await musicRepository.GetArtistByIdAsync(artistId);
        }

        [HttpGet]
        public async Task<ActionResult<Artist[]>> GetByName(string name)
        {
            return await musicRepository.GetArtistByNameAsync(name);
        }

        [HttpPut]
        public async Task<ActionResult<string>> Update(ArtistUpdateRequest req)
        {
            var artist = await musicRepository.GetArtistByIdAsync(req.id);
            if(artist == null)
            {
                return NotFound($"没有Id={req.id}的Artist");
            }
            else
            {
                artist.Update(req.PicUrl, req.Name);
                return Ok("OK");
            }
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult<string>> DeleteById(Guid id)
        {
            var artist = await musicRepository.GetArtistByIdAsync(id);
            if (artist == null)
            {
                return NotFound($"没有Id={id}的Artist");
            }
            //软删除
            artist.SoftDelete();
            return Ok("Ok");
        }
    }
}

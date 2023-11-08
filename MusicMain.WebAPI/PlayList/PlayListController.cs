using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicAdmin.WebAPI;
using MusicInfrastructure;

namespace MusicMain.WebAPI.PlayList
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork(typeof(MusicDBContext))]
    public class PlayListController : ControllerBase
    {

    }
}

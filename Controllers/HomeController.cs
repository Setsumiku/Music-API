using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Music_API.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IMapper mapper)
        {
            _logger = logger;
        }
        //[HttpGet(Name = nameof(GetRoot))]
        //public IActionResult GetRoot()
        //{
        //    RootModel rootModel = new RootModel();
        //    rootModel.Links.Add(
        //        UrlLink("playlists", "/MusicAPIPlaylist/Get", null));

        //    //rootModel.Links.Add(
        //    //    UrlLink("clients", "GetClients", null));

        //    return Ok(rootModel);
        //}
    }
}
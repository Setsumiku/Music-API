namespace Music_API.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public HomeController(ILogger<HomeController> logger, LinkGenerator linkGenerator, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public IActionResult GetRoot()
        {
            return Ok(CreateLinksForHomeController());
        }

        private IEnumerable<Link> CreateLinksForHomeController()
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, action : nameof(MusicAPIAlbumController.Get), controller : "MusicAPIAlbum"),
                "album_controller",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, action : nameof(MusicAPIArtistController.Get), controller:"MusicAPIArtist"),
                "artist_controller",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, action : nameof(MusicAPIPlaylistController.Get), controller:"MusicAPIPlaylist"),
                "playlist_controller",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, action : nameof(MusicAPISongController.Get), controller:"MusicAPISong"),
                "song_controller",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, action : nameof(MusicAPIGenreController.Get), controller:"MusicAPIGenre"),
                "genre_controller",
                "GET")
            };

            return links;
        }
    }
}
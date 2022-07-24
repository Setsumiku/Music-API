using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Music_API.Data.DAL;
using Music_API.Data.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Artist> _artistRepository;
        private readonly IBaseRepository<Playlist> _playlistRepository;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPIController(IMapper mapper, ILogger logger, IBaseRepository<Song> songRepository, 
            IBaseRepository<Playlist> playlistRepository, IBaseRepository<Album> albumRepository, 
            IBaseRepository<Genre> genreRepository, IBaseRepository<Artist> artistRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _artistRepository = artistRepository;
            _playlistRepository = playlistRepository;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
        }
        // GET: api/<MusicAPIController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MusicAPIController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MusicAPIController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MusicAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MusicAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

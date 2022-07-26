//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Music_API.Data.DAL;
//using Music_API.Data.Model;
//using Music_API.DTOs;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace Music_API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MusicAPIAlbumController : ControllerBase
//    {
//        private readonly IMapper _mapper;
//        //private readonly ILogger _logger;
//        private readonly IBaseRepository<Song> _songRepository;
//        private readonly IBaseRepository<Artist> _artistRepository;
//        private readonly IBaseRepository<Playlist> _playlistRepository;
//        private readonly IBaseRepository<Album> _albumRepository;
//        private readonly IBaseRepository<Genre> _genreRepository;

//        public MusicAPIAlbumController(IMapper mapper, IBaseRepository<Song> songRepository,
//            IBaseRepository<Playlist> playlistRepository, IBaseRepository<Album> albumRepository,
//            IBaseRepository<Genre> genreRepository, IBaseRepository<Artist> artistRepository)
//        {
//            _mapper = mapper;
//            //_logger = logger;
//            _songRepository = songRepository;
//            _artistRepository = artistRepository;
//            _playlistRepository = playlistRepository;
//            _albumRepository = albumRepository;
//            _genreRepository = genreRepository;
//        }

//        // GET: api/<MusicAPIAlbumController>/albums
//        [HttpGet("albums")]
//        public async Task<IActionResult> Get()
//            => Ok(_mapper.Map<IEnumerable<AlbumDto>>(await _albumRepository.GetAllAsync()));

//        // GET api/<MusicAPIAlbumController>/albums/{id}
//        [HttpGet("albums/{id}")]
//        public async Task<IActionResult> Get(int id)
//        {
//            var foundAlbum = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id);
//            return foundAlbum != null ? Ok(_mapper.Map<AlbumDto>(foundAlbum))
//                                        : NotFound();
//        }

//        // POST api/<MusicAPIAlbumController>/albums
//        [HttpPost("albums")]
//        public async Task<IActionResult> Add([FromBody] AlbumDto album)
//        {
//            var savedAlbum = await _albumRepository.CreateAsync(_mapper.Map<Album>(album));
//            return Ok(_mapper.Map<AlbumDto>(savedAlbum));
//        }

//        // PUT api/<MusicAPIAlbumController>/albums/{id}
//        [HttpPut("albums/{id}")]
//        public async Task<IActionResult> Update(string id, [FromBody] AlbumDto album)
//        {
//            if (id != album.AlbumId.ToString())
//                return BadRequest();

//            try
//            {
//                var updatedAlbum = await _albumRepository.UpdateAsync(_mapper.Map<Album>(album));
//                return Ok(_mapper.Map<AlbumDto>(updatedAlbum));
//            }
//            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
//            {
//                return NotFound();
//            }
//        }

//        // DELETE api/<MusicAPIAlbumController>/albums/{id}
//        [HttpDelete("albums/{id}")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            try
//            {
//                var albumToDelete = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id);
//                return Ok(await _albumRepository.DeleteAsync(albumToDelete));
//            }
//            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
//            {
//                return NotFound();
//            }
//        }
//    }
//}

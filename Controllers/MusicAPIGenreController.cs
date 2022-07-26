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
//    public class MusicAPIGenreController : ControllerBase
//    {
//        private readonly IMapper _mapper;
//        //private readonly ILogger _logger;
//        private readonly IBaseRepository<Song> _songRepository;
//        private readonly IBaseRepository<Artist> _artistRepository;
//        private readonly IBaseRepository<Playlist> _playlistRepository;
//        private readonly IBaseRepository<Album> _albumRepository;
//        private readonly IBaseRepository<Genre> _genreRepository;

//        public MusicAPIGenreController(IMapper mapper, IBaseRepository<Song> songRepository,
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

//        // GET: api/<MusicAPIGenreController>/albums
//        [HttpGet("genres")]
//        public async Task<IActionResult> Get()
//            => Ok(_mapper.Map<IEnumerable<GenreDto>>(await _genreRepository.GetAllAsync()));

//        // GET api/<MusicAPIGenreController>/albums/{id}
//        [HttpGet("genres/{id}")]
//        public async Task<IActionResult> Get(int id)
//        {
//            var foundGenre = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id);
//            return foundGenre != null ? Ok(_mapper.Map<GenreDto>(foundGenre))
//                                        : NotFound();
//        }

//        // POST api/<MusicAPIGenreController>/albums
//        [HttpPost("genres")]
//        public async Task<IActionResult> Add([FromBody] GenreDto genre)
//        {
//            var savedGenre = await _genreRepository.CreateAsync(_mapper.Map<Genre>(genre));
//            return Ok(_mapper.Map<GenreDto>(savedGenre));
//        }

//        // PUT api/<MusicAPIGenreontroller>/albums/{id}
//        [HttpPut("genres/{id}")]
//        public async Task<IActionResult> Update(string id, [FromBody] GenreDto genre)
//        {
//            if (id != genre.GenreId.ToString())
//                return BadRequest();

//            try
//            {
//                var updatedGenre = await _genreRepository.UpdateAsync(_mapper.Map<Genre>(genre));
//                return Ok(_mapper.Map<GenreDto>(updatedGenre));
//            }
//            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
//            {
//                return NotFound();
//            }
//        }

//        // DELETE api/<MusicAPIGenreController>/albums/{id}
//        [HttpDelete("genres/{id}")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            try
//            {
//                var genreToDelete = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id);
//                return Ok(await _genreRepository.DeleteAsync(genreToDelete));
//            }
//            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
//            {
//                return NotFound();
//            }
//        }
//    }
//}

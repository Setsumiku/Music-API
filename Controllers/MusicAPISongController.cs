using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;
using Music_API.DTOs;

namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPISongController : ControllerBase
    {
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Artist> _artistRepository;
        private readonly IBaseRepository<Playlist> _playlistRepository;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPISongController(IMapper mapper, IBaseRepository<Song> songRepository,
            IBaseRepository<Playlist> playlistRepository, IBaseRepository<Album> albumRepository,
            IBaseRepository<Genre> genreRepository, IBaseRepository<Artist> artistRepository)
        {
            _mapper = mapper;
            //_logger = logger;
            _songRepository = songRepository;
            _artistRepository = artistRepository;
            _playlistRepository = playlistRepository;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
        }

        // GET: api/<MusicAPIController>/songs
        [HttpGet("songs")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<SongReadDto>>(await _songRepository.GetAllAsync(Array.Empty<string>())));

        // GET api/<MusicAPIController>/songs/{id}
        [HttpGet("songs/{id}")]
        public async Task<IActionResult> Get(int id) //TODO: make separate GET and POST DTOs
        {
            var foundSong = await _songRepository.GetSingleByConditionAsync(song => song.SongId == id, Array.Empty<string>());
            return foundSong != null ? Ok(_mapper.Map<SongReadDto>(foundSong))
                                        : NotFound();
        }

        // POST api/<MusicAPIController>/songs
        [HttpPost("songs")]
        public async Task<IActionResult> Add([FromBody] string songName)
        {
            var savedSong = await _songRepository.CreateAsync(new Song() { SongDescription = songName });
            return Ok(_mapper.Map<SongReadDto>(savedSong));
        }

        // PUT api/<MusicAPIController>/songs/{id}
        [HttpPut("songs/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SongDto song)
        {
            song.SongId = Int32.Parse(id);
            if (id != song.SongId.ToString())
                return BadRequest();

            try
            {
                var updatedSong = await _songRepository.UpdateAsync(_mapper.Map<Song>(song));
                return Ok(_mapper.Map<SongReadDto>(updatedSong));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE api/<MusicAPIController>/songs/{id}
        [HttpDelete("songs/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var songToDelete = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id, Array.Empty<string>());
                return Ok(await _songRepository.DeleteAsync(songToDelete));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

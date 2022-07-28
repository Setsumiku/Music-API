using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;
using Music_API.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPIGenreController : ControllerBase
    {
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Artist> _artistRepository;
        private readonly IBaseRepository<Playlist> _playlistRepository;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPIGenreController(IMapper mapper, IBaseRepository<Song> songRepository,
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

        // GET: api/<MusicAPIController>/genres
        [HttpGet("genres")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<GenreReadDto>>(await _genreRepository.GetAllAsync(new string[] { })));

        // GET api/<MusicAPIController>/genres/{id}
        [HttpGet("genres/{id}")]
        public async Task<IActionResult> Get(int id) //TODO: make separate GET and POST DTOs
        {
            var foundGenre = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id, Array.Empty<string>());
            return foundGenre != null ? Ok(_mapper.Map<GenreReadDto>(foundGenre))
                                        : NotFound();
        }

        // POST api/<MusicAPIController>/genres
        [HttpPost("genres")]
        public async Task<IActionResult> Add([FromBody] string genreName)
        {
            var savedGenre = await _genreRepository.CreateAsync(new Genre() { GenreDescription = genreName });
            return Ok(_mapper.Map<GenreReadDto>(savedGenre));
        }

        // PUT api/<MusicAPIController>/genres/{id}
        [HttpPut("genres/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] GenreDto genre)
        {
            genre.GenreId = Int32.Parse(id);
            if (id != genre.GenreId.ToString())
                return BadRequest();

            try
            {
                var updatedGenre = await _genreRepository.UpdateAsync(_mapper.Map<Genre>(genre));
                return Ok(_mapper.Map<GenreDto>(updatedGenre));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE api/<MusicAPIController>/genres/{id}
        [HttpDelete("genres/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var genreToDelete = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, new string[] { "GenreSongs" });
                return Ok(await _genreRepository.DeleteAsync(genreToDelete));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        // GET: api/<MusicAPIController>/genres/{id}/songs
        [HttpGet("genres/{id}/songs")]
        public async Task<IActionResult> GetGenreSongs(int id)
        {
            try
            {
                var genreToUse = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id, new string[] { "GenreSongs" });
                return Ok(_mapper.Map<IEnumerable<SongReadDto>>(genreToUse.GenreSongs));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        // GET api/<MusicAPIController>/genres/{id}/songs/{id}
        [HttpGet("genres/{id}/songs/{id2}")]
        public async Task<IActionResult> GetSingleSongFromGenre(int id, int id2)
        {
            try
            {
                var foundGenre = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id, new string[] { "GenreSongs" });
                return foundGenre != null ? Ok(_mapper.Map<GenreReadDto>(foundGenre.GenreSongs[id2 - 1]))
                                            : NotFound();

            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        // POST api/<MusicAPIController>/genres/{id}/songs
        [HttpPost("genres/{id}/songs")]
        public async Task<IActionResult> AddSong([FromBody] SongDto songDto)
        {
            var songToAdd = new Song();
            _mapper.Map(songDto, songToAdd);
            var savedSong = await _songRepository.CreateAsync(songToAdd);
            return Ok(_mapper.Map<SongReadDto>(savedSong));
        }
        // DELETE api/<MusicAPIController>/genres/{id}/songs
        [HttpDelete("genres/{id}/songs")]
        public async Task<IActionResult> DeleteSong([FromBody] string songName)
        {
            try
            {
                var songToDelete = await _songRepository.GetSingleByConditionAsync(song => song.SongDescription == songName, Array.Empty<string>());
                _ = await _songRepository.DeleteAsync(songToDelete);
                return Ok();
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
            {
                return BadRequest();
            }


        }
        // PUT api/<MusicAPIController>/genres/{id}/songs/{id2}
        [HttpPut("genres/{id}/songs/{id2}")]
        public async Task<IActionResult> PutSongToGenre(string id, string id2)
        {
            var genreToAddSongTo = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, new string[] { "GenreSongs" });
            if (genreToAddSongTo is null)
                return BadRequest();
            var songToAdd = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id2, Array.Empty<string>());
            if (songToAdd is null)
                return BadRequest();
            try
            {
                genreToAddSongTo.GenreSongs.Add(songToAdd);
                var updatedGenre = await _genreRepository.UpdateAsync(genreToAddSongTo);
                return Ok(_mapper.Map<GenreDto>(genreToAddSongTo));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE api/<MusicAPIController>/genres/{id}/songs/{id2}
        [HttpDelete("genres/{id}/songs/{id2}")]
        public async Task<IActionResult> DeleteSongFromGenre(string id, string id2)
        {
            try
            {
                var genreToDeleteFrom = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, new string[] { "GenreSongs" });
                genreToDeleteFrom.GenreSongs.RemoveAt(Int32.Parse(id2) - 1);
                return genreToDeleteFrom != null ? Ok(_mapper.Map<GenreDto>(await _genreRepository.UpdateAsync(genreToDeleteFrom)))
                    : BadRequest();
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentOutOfRangeException || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

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
    public class MusicAPIGenreController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPIGenreController(IMapper mapper, ILogger<MusicAPIGenreController> logger, IBaseRepository<Song> songRepository, IBaseRepository<Genre> genreRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _genreRepository = genreRepository;
        }
        /// <summary>
        /// Use to receive all Genres
        /// </summary>
        /// <returns>Genres</returns>
        // GET: api/<MusicAPIController>/genres
        [HttpGet("genres")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<GenreReadDto>>(await _genreRepository.GetAllAsync(Array.Empty<string>())));
        /// <summary>
        /// Use to receive specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <returns>Genre</returns>
        // GET api/<MusicAPIController>/genres/{id}
        [HttpGet("genres/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var foundGenre = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id, Array.Empty<string>());
            return foundGenre != null ? Ok(_mapper.Map<GenreReadDto>(foundGenre))
                                        : NotFound();
        }
        /// <summary>
        /// Use to Create a new Genre
        /// </summary>
        /// <param name="genreName">String for Genre Description</param>
        /// <returns>Created Genre</returns>
        // POST api/<MusicAPIController>/genres
        [HttpPost("genres")]
        public async Task<IActionResult> Add([FromBody] string genreName)
        {
            var savedGenre = await _genreRepository.CreateAsync(new Genre() { GenreDescription = genreName });
            return Ok(_mapper.Map<GenreReadDto>(savedGenre));
        }
        /// <summary>
        /// Use to Edit Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="genre">String for new name for the Genre</param>
        /// <returns>Updated Genre</returns>
        // PUT api/<MusicAPIController>/genres/{id}
        [HttpPut("genres/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] GenreDto genre)
        {
            try
            {
                genre.GenreId = Int32.Parse(id);
            }
            catch (Exception ex) when (ex is FormatException)
            {
                return BadRequest();
            }
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
        /// <summary>
        /// Use to Remove Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/genres/{id}
        [HttpDelete("genres/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var genreToDelete = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, Array.Empty<string>());
                return Ok(await _genreRepository.DeleteAsync(genreToDelete));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Use to Get songs from specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <returns>Songs from the Genre</returns>
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
        /// <summary>
        /// Use to Get specific Song from specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Song from the Genre</returns>
        // GET api/<MusicAPIController>/genres/{id}/songs/{id2}
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
        /// <summary>
        /// Use to Add specific song to a specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Updated Genre</returns>
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
        /// <summary>
        /// Use to Remove song from a Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Code for success or failure</returns>
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

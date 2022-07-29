using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;

namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPIGenreController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPIGenreController(IMapper mapper, ILogger<MusicAPIGenreController> logger, IBaseRepository<Album> albumRepository, IBaseRepository<Genre> genreRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _albumRepository = albumRepository;
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
        public async Task<IActionResult> Update(string id, [FromBody] GenreReadDto genre)
        {
            try
            {
                var genreToUpdate = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, Array.Empty<string>());
                if (genreToUpdate != null)
                {
                    genreToUpdate.GenreDescription = genre.GenreDescription;
                    _ = await _genreRepository.UpdateAsync(genreToUpdate);
                    return Ok();
                }
                else return NotFound();
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
                _ = await _genreRepository.DeleteAsync(genreToDelete);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException || e is NullReferenceException)
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Use to Get albums from specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <returns>Albums from the Genre</returns>
        // GET: api/<MusicAPIController>/genres/{id}/albums
        [HttpGet("genres/{id}/albums")]
        public async Task<IActionResult> GetGenreAlbums(int id)
        {
            try
            {
                var genreToUse = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id, new string[] { "GenreAlbums" });
                return genreToUse is not null ? Ok(_mapper.Map<IEnumerable<AlbumReadDto>>(genreToUse.GenreAlbums))
                    : NotFound();
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Use to Get specific Album from specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="id2">String for ID of Album</param>
        /// <returns>Album from the Genre</returns>
        // GET api/<MusicAPIController>/genres/{id}/albums/{id2}
        [HttpGet("genres/{id}/albums/{id2}")]
        public async Task<IActionResult> GetSingleAlbumFromGenre(int id, int id2)
        {
            try
            {
                var foundGenre = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId == id, new string[] { "GenreAlbums" });
                var albumFromGenre = foundGenre.GenreAlbums[id2 - 1];
                return foundGenre is not null ? Ok(_mapper.Map<GenreReadDto>(albumFromGenre))
                      : NotFound();

            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is OverflowException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Use to Add specific Album to a specific Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="id2">String for ID of Album</param>
        /// <returns>Updated Genre</returns>
        // PUT api/<MusicAPIController>/genres/{id}/albums/{id2}
        [HttpPut("genres/{id}/albums/{id2}")]
        public async Task<IActionResult> PutAlbumToGenre(string id, string id2)
        {
            var genreToAddAlbumTo = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, new string[] { "GenreSongs" });
            if (genreToAddAlbumTo is null)
                return NotFound();
            var albumToAdd = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id2, Array.Empty<string>());
            if (albumToAdd is null)
                return NotFound();
            try
            {
                genreToAddAlbumTo.GenreAlbums.Add(albumToAdd);
                var updatedGenre = await _genreRepository.UpdateAsync(genreToAddAlbumTo);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Use to Remove Album from a Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="id2">String for ID of Album</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/genres/{id}/albums/{id2}
        [HttpDelete("genres/{id}/albums/{id2}")]
        public async Task<IActionResult> DeleteAlbumFromGenre(string id, string id2)
        {
            try
            {
                var genreToDeleteFrom = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, new string[] { "GenreAlbums" });
                if (genreToDeleteFrom is null) return NotFound();
                genreToDeleteFrom.GenreAlbums.RemoveAt(Int32.Parse(id2) - 1);
                _ = await _genreRepository.UpdateAsync(genreToDeleteFrom);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is OverflowException || e is ArgumentOutOfRangeException
            || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;
using Music_API.Entities;

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
        private readonly LinkGenerator _linkGenerator;

        public MusicAPIGenreController(IMapper mapper, LinkGenerator linkGenerator, ILogger<MusicAPIGenreController> logger, IBaseRepository<Album> albumRepository, IBaseRepository<Genre> genreRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Use to receive all Genres
        /// </summary>
        /// <returns>Genres</returns>
        // GET: api/<MusicAPIController>/genres
        [HttpGet("genres")]
        public async Task<IActionResult> Get()
        {
            var genres = _mapper.Map<IEnumerable<GenreReadDto>>(await _genreRepository.GetAllAsync(Array.Empty<string>()));
            for (var index = 0; index < genres.Count(); index++)
            {
                genres.ElementAt(index).Add("Name", new { genres.ElementAt(index).GenreDescription });
                var genreLinks = CreateLinksForGenre(genres.ElementAt(index).GenreId);
                genres.ElementAt(index).Add("Links", genreLinks);
            }
            var genresWrapper = new LinkCollectionWrapper<GenreReadDto>(genres);
            return Ok(CreateLinksForGenres(genresWrapper));
        }

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
            if (foundGenre is null) return NotFound();
            var mappedGenre = _mapper.Map<GenreReadDto>(foundGenre);
            mappedGenre.Add("Name", new { mappedGenre.GenreDescription });
            var albumLink = new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetGenreAlbums), values: new { id }),
                "get_genre_albums",
                "GET");
            mappedGenre.Add("Links", CreateLinksForGenre(foundGenre.GenreId, "", albumLink));
            return Ok(mappedGenre);
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
            return Created("api/MusicAPIGenre/genres/" + savedGenre.GenreId, _mapper.Map<GenreReadDto>(savedGenre));
        }

        /// <summary>
        /// Use to Edit Genre
        /// </summary>
        /// <param name="id">String for ID of Genre</param>
        /// <param name="genreName">String for new name for the Genre</param>
        /// <returns>Updated Genre</returns>
        // PUT api/<MusicAPIController>/genres/{id}
        [HttpPut("genres/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string genreName)
        {
            try
            {
                var genreToUpdate = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, Array.Empty<string>());
                if (genreToUpdate != null)
                {
                    genreToUpdate.GenreDescription = genreName;
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
                if (genreToUse is null) return NotFound();
                var albumList = _mapper.Map<IEnumerable<AlbumReadDto>>(genreToUse.GenreAlbums);
                for (var index = 0; index < albumList.Count(); index++)
                {
                    albumList.ElementAt(index).Add("Name", new { albumList.ElementAt(index).AlbumDescription });
                    var albumLinks = CreateLinksForAlbum(id, index + 1);
                    albumList.ElementAt(index).Add("Links", albumLinks);
                }
                var albumsWrapper = new LinkCollectionWrapper<AlbumReadDto>(albumList);
                return Ok(CreateLinksForAlbums(albumsWrapper));
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
                if (foundGenre is null) return NotFound();
                var albumFromGenre = foundGenre.GenreAlbums[id2 - 1];
                var mappedAlbum = _mapper.Map<AlbumReadDto>(albumFromGenre);
                mappedAlbum.Add("Name", new { mappedAlbum.AlbumDescription });
                mappedAlbum.Add("Links", CreateLinksForAlbum(id, id2));
                return Ok(mappedAlbum);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is OverflowException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
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
            var genreToAddAlbumTo = await _genreRepository.GetSingleByConditionAsync(genre => genre.GenreId.ToString() == id, new string[] { "GenreAlbums" });
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

        private IEnumerable<Link> CreateLinksForGenre(int id, string fields = "", Link optional = null)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id, fields }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }),
                "delete_genre",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }),
                "update_genre",
                "PUT")
            };
            if (optional is not null) links.Add(optional);

            return links;
        }

        private IEnumerable<Link> CreateLinksForAlbum(int id, int id2, Link optional = null)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetSingleAlbumFromGenre), values: new { id, id2 }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteAlbumFromGenre), values: new { id, id2 }),
                "remove_album_from_genre",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(PutAlbumToGenre), values: new { id, id2 }),
                "add_existing_album_to_genre",
                "PUT")
            };
            if (optional is not null) links.Add(optional);

            return links;
        }

        private LinkCollectionWrapper<GenreReadDto> CreateLinksForGenres(LinkCollectionWrapper<GenreReadDto> genresWrapper)
        {
            genresWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { }),
                    "self",
                    "GET"));

            return genresWrapper;
        }

        private LinkCollectionWrapper<AlbumReadDto> CreateLinksForAlbums(LinkCollectionWrapper<AlbumReadDto> albumsWrapper)
        {
            albumsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetGenreAlbums), values: new { }),
                    "self",
                    "GET"));

            return albumsWrapper;
        }
    }
}
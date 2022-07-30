using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;
using Music_API.Entities;

namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPIAlbumController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly LinkGenerator _linkGenerator;

        public MusicAPIAlbumController(IMapper mapper, LinkGenerator linkGenerator, ILogger<MusicAPIAlbumController> logger, IBaseRepository<Song> songRepository, IBaseRepository<Album> albumRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _albumRepository = albumRepository;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Use to receive all Albums
        /// </summary>
        /// <returns>Albums</returns>
        // GET: api/<MusicAPIController>/albums
        [HttpGet("albums")]
        public async Task<IActionResult> Get()
        {
            var albums = _mapper.Map<IEnumerable<AlbumReadDto>>(await _albumRepository.GetAllAsync(Array.Empty<string>()));
            for (var index = 0; index < albums.Count(); index++)
            {
                albums.ElementAt(index).Add("Name", new { albums.ElementAt(index).AlbumDescription });
                var albumLinks = CreateLinksForAlbum(albums.ElementAt(index).AlbumId);
                albums.ElementAt(index).Add("Links", albumLinks);
            }
            var albumsWrapper = new LinkCollectionWrapper<AlbumReadDto>(albums);
            return Ok(CreateLinksForPlaylists(albumsWrapper));
        }

        /// <summary>
        /// Use to receive specific Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <returns>Album</returns>
        // GET api/<MusicAPIController>/albums/{id}
        [HttpGet("albums/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var foundAlbum = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, Array.Empty<string>());
            if (foundAlbum is null) return NotFound();
            var mappedAlbum = _mapper.Map<AlbumReadDto>(foundAlbum);
            mappedAlbum.Add("Name", new { mappedAlbum.AlbumDescription });
            var songLink = new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAlbumSongs), values: new { id }),
                "get_album_songs",
                "GET");
            mappedAlbum.Add("Links", CreateLinksForAlbum(foundAlbum.AlbumId, "", songLink));
            return Ok(mappedAlbum);
        }

        /// <summary>
        /// Use to Create a new Album
        /// </summary>
        /// <param name="albumName">String for Album Description</param>
        /// <returns>Created Album</returns>
        // POST api/<MusicAPIController>/albums
        [HttpPost("albums")]
        public async Task<IActionResult> Add([FromBody] string albumName)
        {
            var savedAlbum = await _albumRepository.CreateAsync(new Album() { AlbumDescription = albumName });
            return Created("api/MusicAPIAlbum/albums/" + savedAlbum.AlbumId, _mapper.Map<AlbumReadDto>(savedAlbum));
        }

        /// <summary>
        /// Use to Edit Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <param name="albumName">String for new name for the Album</param>
        /// <returns>Updated Album</returns>
        // PUT api/<MusicAPIController>/albums/{id}
        [HttpPut("albums/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string albumName)
        {
            try
            {
                var albumToUpdate = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, Array.Empty<string>());
                if (albumToUpdate is not null)
                {
                    albumToUpdate.AlbumDescription = albumName;
                    _ = await _albumRepository.UpdateAsync(albumToUpdate);
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
        /// Use to Remove Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/albums/{id}
        [HttpDelete("albums/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var albumToDelete = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs" });
                _ = await _albumRepository.DeleteAsync(albumToDelete);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Get songs from specific Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <returns>Songs from the Album</returns>
        // GET: api/<MusicAPIController>/albums/{id}/songs
        [HttpGet("albums/{id}/songs")]
        public async Task<IActionResult> GetAlbumSongs(int id)
        {
            try
            {
                var albumToUse = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, new string[] { "AlbumSongs" });
                if (albumToUse is null) return NotFound();
                var songList = _mapper.Map<IEnumerable<SongReadDto>>(albumToUse.AlbumSongs);
                for (var index = 0; index < songList.Count(); index++)
                {
                    songList.ElementAt(index).Add("Name", new { songList.ElementAt(index).SongDescription });
                    var songLinks = CreateLinksForSong(id, index + 1);
                    songList.ElementAt(index).Add("Links", songLinks);
                }
                var songsWrapper = new LinkCollectionWrapper<SongReadDto>(songList);
                return Ok(CreateLinksForSongs(songsWrapper));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Get specific Song from specific Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Song from the Album</returns>
        // GET api/<MusicAPIController>/albums/{id}/songs/{id2}
        [HttpGet("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> GetSingleSongFromAlbum(int id, int id2)
        {
            try
            {
                var foundAlbum = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, new string[] { "AlbumSongs" });
                if (foundAlbum is null) return NotFound();
                var songFromAlbum = foundAlbum.AlbumSongs[id2 - 1];
                var mappedSong = _mapper.Map<SongReadDto>(songFromAlbum);
                mappedSong.Add("Name", new { mappedSong.SongDescription });
                mappedSong.Add("Links", CreateLinksForSong(id, id2));
                return Ok(mappedSong);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is OverflowException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Add specific song to a specific Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Updated Album</returns>
        // PUT api/<MusicAPIController>/albums/{id}/songs/{id2}
        [HttpPut("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> PutSongToAlbum(string id, string id2)
        {
            var albumToAddSongTo = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs" });
            if (albumToAddSongTo is null)
                return NotFound();
            var songToAdd = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id2, Array.Empty<string>());
            if (songToAdd is null)
                return NotFound();
            try
            {
                albumToAddSongTo.AlbumSongs.Add(songToAdd);
                var updatedAlbum = await _albumRepository.UpdateAsync(albumToAddSongTo);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Remove song from an Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/albums/{id}/songs/{id2}
        [HttpDelete("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> DeleteSongFromAlbum(string id, string id2)
        {
            try
            {
                var albumToDeleteFrom = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs" });
                if (albumToDeleteFrom is null) return NotFound();
                albumToDeleteFrom.AlbumSongs.RemoveAt(Int32.Parse(id2) - 1);
                _ = await _albumRepository.UpdateAsync(albumToDeleteFrom);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is OverflowException || e is ArgumentOutOfRangeException
            || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        private IEnumerable<Link> CreateLinksForAlbum(int id, string fields = "", Link optional = null)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id, fields }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }),
                "delete_album",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }),
                "update_album",
                "PUT")
            };
            if (optional is not null) links.Add(optional);

            return links;
        }

        private IEnumerable<Link> CreateLinksForSong(int id, int id2, Link optional = null)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetSingleSongFromAlbum), values: new { id, id2 }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteSongFromAlbum), values: new { id, id2 }),
                "remove_song_from_album",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(PutSongToAlbum), values: new { id, id2 }),
                "add_existing_song_to_album",
                "PUT")
            };
            if (optional is not null) links.Add(optional);

            return links;
        }

        private LinkCollectionWrapper<AlbumReadDto> CreateLinksForPlaylists(LinkCollectionWrapper<AlbumReadDto> albumWrapper)
        {
            albumWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { }),
                    "self",
                    "GET"));

            return albumWrapper;
        }

        private LinkCollectionWrapper<SongReadDto> CreateLinksForSongs(LinkCollectionWrapper<SongReadDto> songsWrapper)
        {
            songsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAlbumSongs), values: new { }),
                    "self",
                    "GET"));

            return songsWrapper;
        }
    }
}
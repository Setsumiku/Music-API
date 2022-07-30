namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPIPlaylistController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Playlist> _playlistRepository;
        private readonly LinkGenerator _linkGenerator;

        public MusicAPIPlaylistController(IMapper mapper, ILogger<MusicAPIPlaylistController> logger, LinkGenerator linkGenerator,
            IBaseRepository<Song> songRepository, IBaseRepository<Playlist> playlistRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _playlistRepository = playlistRepository;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Use to receive all Playlists
        /// </summary>
        /// <returns>Playlists</returns>
        // GET: api/<MusicAPIController>/playlists
        [HttpGet("playlists")]
        public async Task<IActionResult> Get()
        {
            var playlists = _mapper.Map<IEnumerable<PlaylistReadDto>>(await _playlistRepository.GetAllAsync(Array.Empty<string>()));
            for (var index = 0; index < playlists.Count(); index++)
            {
                playlists.ElementAt(index).Add("Name", new { playlists.ElementAt(index).PlaylistDescription });
                var playlistLinks = CreateLinksForPlaylist(playlists.ElementAt(index).PlaylistId);
                playlists.ElementAt(index).Add("Links", playlistLinks);
            }
            var playlistsWrapper = new LinkCollectionWrapper<PlaylistReadDto>(playlists);
            return Ok(CreateLinksForPlaylists(playlistsWrapper));
        }

        /// <summary>
        /// Use to receive specific Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <returns>Playlist</returns>
        // GET api/<MusicAPIController>/playlists/{id}
        [HttpGet("playlists/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var foundPlaylist = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId == id, Array.Empty<string>());
            if (foundPlaylist is null) return NotFound();
            var mappedPlaylist = _mapper.Map<PlaylistReadDto>(foundPlaylist);
            mappedPlaylist.Add("Name", new { mappedPlaylist.PlaylistDescription });
            var songLink = new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetPlaylistSongs), values: new { id }),
                "get_playlist_songs",
                "GET");
            mappedPlaylist.Add("Links", CreateLinksForPlaylist(foundPlaylist.PlaylistId, "", songLink));
            return Ok(mappedPlaylist);
        }

        /// <summary>
        /// Use to Create a new Playlist
        /// </summary>
        /// <param name="playlistName">String for Playlist Description</param>
        /// <returns>Created Playlist</returns>
        // POST api/<MusicAPIController>/playlists
        [HttpPost("playlists")]
        public async Task<IActionResult> Add([FromBody] string playlistName)
        {
            var savedPlaylist = await _playlistRepository.CreateAsync(new Playlist() { PlaylistDescription = playlistName });
            return Created("api/MusicAPIPlaylist/playlists/" + savedPlaylist.PlaylistId, _mapper.Map<PlaylistReadDto>(savedPlaylist));
        }

        /// <summary>
        /// Use to Edit Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <param name="playlistName">String for new name for the Playlist</param>
        /// <returns>Updated Playlist</returns>
        // PUT api/<MusicAPIController>/playlists/{id}
        [HttpPut("playlists/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string playlistName)
        {
            try
            {
                var playlistToUpdate = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId.ToString() == id, Array.Empty<string>());
                if (playlistToUpdate != null)
                {
                    playlistToUpdate.PlaylistDescription = playlistName;
                    _ = await _playlistRepository.UpdateAsync(playlistToUpdate);
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
        /// Use to Remove Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/playlists/{id}
        [HttpDelete("playlists/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var playlistToDelete = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId.ToString() == id, new string[] { "PlaylistSongs" });
                _ = await _playlistRepository.DeleteAsync(playlistToDelete);
                return Ok();
            }
            catch (Exception e) when (e is NullReferenceException || e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Get songs from specific Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <returns>Songs from the Playlist</returns>
        // GET: api/<MusicAPIController>/playlists/{id}/songs
        [HttpGet("playlists/{id}/songs")]
        public async Task<IActionResult> GetPlaylistSongs(int id)
        {
            try
            {
                var playlistToUse = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId == id, new string[] { "PlaylistSongs" });
                if (playlistToUse is null) return NotFound();
                var songList = _mapper.Map<IEnumerable<SongReadDto>>(playlistToUse.PlaylistSongs);
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
        /// Use to Get specific Song from specific Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Song from the Playlist</returns>
        // GET api/<MusicAPIController>/playlists/{id}/songs/{id}
        [HttpGet("playlists/{id}/songs/{id2}")]
        public async Task<IActionResult> GetSingleSongFromPlaylist(int id, int id2)
        {
            try
            {
                var foundPlaylist = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId == id, new string[] { "PlaylistSongs" });
                if (foundPlaylist is null) return NotFound();
                var songFromPlaylist = foundPlaylist.PlaylistSongs[id2 - 1];
                var mappedSong = _mapper.Map<SongReadDto>(songFromPlaylist);
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
        /// Use to Add specific song to a specific Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Updated Playlist</returns>
        // PUT api/<MusicAPIController>/playlists/{id}/songs/{id2}
        [HttpPut("playlists/{id}/songs/{id2}")]
        public async Task<IActionResult> PutSongToPlaylist(string id, string id2)
        {
            var playlistToAddSongTo = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId.ToString() == id, new string[] { "PlaylistSongs" });
            if (playlistToAddSongTo is null)
                return NotFound();
            var songToAdd = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id2, Array.Empty<string>());
            if (songToAdd is null)
                return NotFound();
            try
            {
                playlistToAddSongTo.PlaylistSongs.Add(songToAdd);
                var updatedPlaylist = await _playlistRepository.UpdateAsync(playlistToAddSongTo);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Remove song from a Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <param name="id2">String for ID of Song</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/playlists/{id}/songs/{id2}
        [HttpDelete("playlists/{id}/songs/{id2}")]
        public async Task<IActionResult> DeleteSongFromPlaylist(string id, string id2)
        {
            try
            {
                var playlistToDeleteFrom = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId.ToString() == id, new string[] { "PlaylistSongs" });
                if (playlistToDeleteFrom is null) return NotFound();
                playlistToDeleteFrom.PlaylistSongs.RemoveAt(Int32.Parse(id2) - 1);
                _ = await _playlistRepository.UpdateAsync(playlistToDeleteFrom);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is OverflowException || e is ArgumentOutOfRangeException
            || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        private IEnumerable<Link> CreateLinksForPlaylist(int id, string fields = "", Link optional = null)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id, fields }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }),
                "delete_playlist",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }),
                "update_playlist",
                "PUT")
            };
            if (optional is not null) links.Add(optional);

            return links;
        }

        private IEnumerable<Link> CreateLinksForSong(int id, int id2, Link optional = null)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetSingleSongFromPlaylist), values: new { id, id2 }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteSongFromPlaylist), values: new { id, id2 }),
                "remove_song_from_playlist",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(PutSongToPlaylist), values: new { id, id2 }),
                "add_existing_song_to_playlist",
                "PUT")
            };
            if (optional is not null) links.Add(optional);

            return links;
        }

        private LinkCollectionWrapper<PlaylistReadDto> CreateLinksForPlaylists(LinkCollectionWrapper<PlaylistReadDto> playlistsWrapper)
        {
            playlistsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { }),
                    "self",
                    "GET"));

            return playlistsWrapper;
        }

        private LinkCollectionWrapper<SongReadDto> CreateLinksForSongs(LinkCollectionWrapper<SongReadDto> songsWrapper)
        {
            songsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetPlaylistSongs), values: new { }),
                    "self",
                    "GET"));

            return songsWrapper;
        }
    }
}
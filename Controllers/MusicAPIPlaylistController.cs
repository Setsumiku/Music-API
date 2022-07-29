using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;

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

        public MusicAPIPlaylistController(IMapper mapper, ILogger<MusicAPIPlaylistController> logger, IBaseRepository<Song> songRepository, IBaseRepository<Playlist> playlistRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _playlistRepository = playlistRepository;
        }
        /// <summary>
        /// Use to receive all Playlists
        /// </summary>
        /// <returns>Playlists</returns>
        // GET: api/<MusicAPIController>/playlists
        [HttpGet("playlists")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<PlaylistReadDto>>(await _playlistRepository.GetAllAsync(new string[] { })));
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
            return foundPlaylist != null ? Ok(_mapper.Map<PlaylistReadDto>(foundPlaylist))
                                        : NotFound();
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
            return Ok(_mapper.Map<PlaylistReadDto>(savedPlaylist));
        }
        /// <summary>
        /// Use to Edit Playlist
        /// </summary>
        /// <param name="id">String for ID of Playlist</param>
        /// <param name="playlist">String for new name for the Playlist</param>
        /// <returns>Updated Playlist</returns>
        // PUT api/<MusicAPIController>/playlists/{id}
        [HttpPut("playlists/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PlaylistDto playlist)
        {
            playlist.PlaylistId = Int32.Parse(id);
            if (id != playlist.PlaylistId.ToString())
                return BadRequest();

            try
            {
                var updatedPlaylist = await _playlistRepository.UpdateAsync(_mapper.Map<Playlist>(playlist));
                return Ok(_mapper.Map<PlaylistDto>(updatedPlaylist));
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
                return Ok(await _playlistRepository.DeleteAsync(playlistToDelete));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
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
                return Ok(_mapper.Map<IEnumerable<SongReadDto>>(playlistToUse.PlaylistSongs));
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
                return foundPlaylist != null ? Ok(_mapper.Map<SongReadDto>(foundPlaylist.PlaylistSongs[id2 - 1]))
                                            : NotFound();

            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
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
                return BadRequest();
            var songToAdd = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id2, Array.Empty<string>());
            if (songToAdd is null)
                return BadRequest();
            try
            {
                playlistToAddSongTo.PlaylistSongs.Add(songToAdd);
                var updatedPlaylist = await _playlistRepository.UpdateAsync(playlistToAddSongTo);
                return Ok(_mapper.Map<PlaylistDto>(updatedPlaylist));
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
                playlistToDeleteFrom.PlaylistSongs.RemoveAt(Int32.Parse(id2)-1);
                return playlistToDeleteFrom != null ? Ok(_mapper.Map<PlaylistDto>(await _playlistRepository.UpdateAsync(playlistToDeleteFrom)))
                    : BadRequest();
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentOutOfRangeException || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

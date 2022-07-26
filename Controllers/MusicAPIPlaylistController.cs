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
    public class MusicAPIPlaylistController : ControllerBase
    {
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Artist> _artistRepository;
        private readonly IBaseRepository<Playlist> _playlistRepository;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPIPlaylistController(IMapper mapper, IBaseRepository<Song> songRepository,
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

        // GET: api/<MusicAPIController>/playlists
        [HttpGet("playlists")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<PlaylistReadDto>>(await _playlistRepository.GetAllAsync(new string[] { })));

        // GET api/<MusicAPIController>/playlists/{id}
        [HttpGet("playlists/{id}")]
        public async Task<IActionResult> Get(int id) //TODO: make separate GET and POST DTOs
        {
            var foundPlaylist = await _playlistRepository.GetSingleByConditionAsync(playlist => playlist.PlaylistId == id, Array.Empty<string>());
            return foundPlaylist != null ? Ok(_mapper.Map<PlaylistReadDto>(foundPlaylist))
                                        : NotFound();
        }

        // POST api/<MusicAPIController>/playlists
        [HttpPost("playlists")]
        public async Task<IActionResult> Add([FromBody] string playlistName)
        {
            var savedPlaylist = await _playlistRepository.CreateAsync(new Playlist() { PlaylistDescription = playlistName });
            return Ok(_mapper.Map<PlaylistReadDto>(savedPlaylist));
        }

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
        // POST api/<MusicAPIController>/playlists/{id}/songs
        [HttpPost("playlists/{id}/songs")]
        public async Task<IActionResult> AddSong([FromBody] string songName)
        {
            var savedSong = await _songRepository.CreateAsync(new Song() { SongDescription = songName });
            return Ok(_mapper.Map<SongReadDto>(savedSong));
        }
        // DELETE api/<MusicAPIController>/playlists/{id}/songs
        [HttpDelete("playlists/{id}/songs")]
        public async Task<IActionResult> DeleteSong([FromBody] string songName)
        {
            try
            {
                var songToDelete = await _songRepository.GetSingleByConditionAsync(song => song.SongDescription == songName, Array.Empty<string>());
                _ = await _songRepository.DeleteAsync(songToDelete);
                return Ok();
            }
            catch ( Exception ex) when(ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
            {
                return BadRequest();
            }


        }
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

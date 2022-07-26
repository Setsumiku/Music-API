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
    public class MusicAPIAlbumController : ControllerBase
    {
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly IBaseRepository<Artist> _artistRepository;
        private readonly IBaseRepository<Playlist> _playlistRepository;
        private readonly IBaseRepository<Album> _albumRepository;
        private readonly IBaseRepository<Genre> _genreRepository;

        public MusicAPIAlbumController(IMapper mapper, IBaseRepository<Song> songRepository,
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

        // GET: api/<MusicAPIController>/albums
        [HttpGet("albums")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<GenreReadDto>>(await _genreRepository.GetAllAsync(new string[] { })));

        // GET api/<MusicAPIController>/albums/{id}
        [HttpGet("albums/{id}")]
        public async Task<IActionResult> Get(int id) //TODO: make separate GET and POST DTOs
        {
            var foundAlbum = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, Array.Empty<string>());
            return foundAlbum != null ? Ok(_mapper.Map<AlbumReadDto>(foundAlbum))
                                        : NotFound();
        }

        // POST api/<MusicAPIController>/albums
        [HttpPost("albums")]
        public async Task<IActionResult> Add([FromBody] string albumName)
        {
            var savedAlbum = await _albumRepository.CreateAsync(new Album() { AlbumDescription = albumName });
            return Ok(_mapper.Map<AlbumReadDto>(savedAlbum));
        }

        // PUT api/<MusicAPIController>/albums/{id}
        [HttpPut("albums/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AlbumDto album)
        {
            album.AlbumId = Int32.Parse(id);
            if (id != album.AlbumId.ToString())
                return BadRequest();

            try
            {
                var updatedAlbum = await _albumRepository.UpdateAsync(_mapper.Map<Album>(album));
                return Ok(_mapper.Map<AlbumDto>(updatedAlbum));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE api/<MusicAPIController>/albums/{id}
        [HttpDelete("albums/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var albumToDelete = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs" });
                return Ok(await _albumRepository.DeleteAsync(albumToDelete));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        // GET: api/<MusicAPIController>/albums/{id}/songs
        [HttpGet("albums/{id}/songs")]
        public async Task<IActionResult> GetAlbumSongs(int id)
        {
            try
            {
                var albumToUse = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, new string[] { "AlbumSongs" });
                return Ok(_mapper.Map<IEnumerable<AlbumReadDto>>(albumToUse.AlbumSongs));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        // GET api/<MusicAPIController>/albums/{id}/songs/{id}
        [HttpGet("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> GetSingleSongFromAlbum(int id, int id2)
        {
            try
            {
                var foundAlbum = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, new string[] { "AlbumSongs" });
                return foundAlbum != null ? Ok(_mapper.Map<AlbumReadDto>(foundAlbum.AlbumSongs[id2 - 1]))
                                            : NotFound();

            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
        // POST api/<MusicAPIController>/albums/{id}/songs
        [HttpPost("albums/{id}/songs")]
        public async Task<IActionResult> AddSong([FromBody] string songName)
        {
            var savedSong = await _songRepository.CreateAsync(new Song() { SongDescription = songName });
            return Ok(_mapper.Map<SongReadDto>(savedSong));
        }
        // DELETE api/<MusicAPIController>/albums/{id}/songs
        [HttpDelete("albums/{id}/songs")]
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
        // PUT api/<MusicAPIController>/albums/{id}/songs/{id2}
        [HttpPut("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> PutSongToAlbum(string id, string id2)
        {
            var albumToAddSongTo = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs" });
            if (albumToAddSongTo is null)
                return BadRequest();
            var songToAdd = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id2, Array.Empty<string>());
            if (songToAdd is null)
                return BadRequest();
            try
            {
                albumToAddSongTo.AlbumSongs.Add(songToAdd);
                var updatedAlbum = await _albumRepository.UpdateAsync(albumToAddSongTo);
                return Ok(_mapper.Map<AlbumDto>(albumToAddSongTo));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE api/<MusicAPIController>/albums/{id}/songs/{id2}
        [HttpDelete("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> DeleteSongFromAlbum(string id, string id2)
        {
            try
            {
                var albumToDeleteFrom = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs" });
                albumToDeleteFrom.AlbumSongs.RemoveAt(Int32.Parse(id2) - 1);
                return albumToDeleteFrom != null ? Ok(_mapper.Map<AlbumDto>(await _albumRepository.UpdateAsync(albumToDeleteFrom)))
                    : BadRequest();
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentOutOfRangeException || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

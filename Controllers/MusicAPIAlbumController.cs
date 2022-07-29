using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_API.Data.DAL;
using Music_API.Data.Model;
using Music_API.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

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

        public MusicAPIAlbumController(IMapper mapper, ILogger<MusicAPIAlbumController> logger, IBaseRepository<Song> songRepository, IBaseRepository<Album> albumRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _albumRepository = albumRepository;
        }
        /// <summary>
        /// Use to receive all Albums
        /// </summary>
        /// <returns>Albums</returns>
        // GET: api/<MusicAPIController>/albums
        [HttpGet("albums")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Found albums")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<AlbumReadDto>>(await _albumRepository.GetAllAsync(new string[] { })));

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
            return foundAlbum is not null ? Ok(_mapper.Map<AlbumReadDto>(foundAlbum))
                                        : NotFound();
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
            return Created("api/MusicAPIAlbum/albums/"+savedAlbum.AlbumId,_mapper.Map<AlbumReadDto>(savedAlbum));
        }
        /// <summary>
        /// Use to Edit Album
        /// </summary>
        /// <param name="id">String for ID of Album</param>
        /// <param name="album">String for new name for the Album</param>
        /// <returns>Updated Album</returns>
        // PUT api/<MusicAPIController>/albums/{id}
        [HttpPut("albums/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AlbumReadDto album)
        {
            try
            {
                var albumToUpdate = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, Array.Empty<string>());
                if (albumToUpdate != null)
                {
                    albumToUpdate.AlbumDescription = album.AlbumDescription;
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
                var albumToDelete = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId.ToString() == id, new string[] { "AlbumSongs"});
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
                return albumToUse is not null ? Ok(_mapper.Map<IEnumerable<SongReadDto>>(albumToUse.AlbumSongs)) : 
                    NotFound();
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
        // GET api/<MusicAPIController>/albums/{id}/songs/{id}
        [HttpGet("albums/{id}/songs/{id2}")]
        public async Task<IActionResult> GetSingleSongFromAlbum(int id, int id2)
        {
            try
            {
                var foundAlbum = await _albumRepository.GetSingleByConditionAsync(album => album.AlbumId == id, new string[] { "AlbumSongs" });
                var songFromAlbum = foundAlbum.AlbumSongs[id2 - 1];
                return (foundAlbum is not null) ? Ok(_mapper.Map<SongReadDto>(songFromAlbum)) 
                    : NotFound();
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is NullReferenceException || ex is DbUpdateConcurrencyException)
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
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentOutOfRangeException || e is FormatException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

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
    public class MusicAPIArtistController : ControllerBase
    {
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;
        private readonly IBaseRepository<Artist> _artistRepository;

        public MusicAPIArtistController(IMapper mapper, IBaseRepository<Artist> artistRepository)
        {
            _mapper = mapper;
            //_logger = logger;
            _artistRepository = artistRepository;
        }
        // GET: api/<MusicAPIController>/artists
        [HttpGet("artists")]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<IEnumerable<ArtistReadDto>>(await _artistRepository.GetAllAsync(Array.Empty<string>())));

        // GET api/<MusicAPIController>/artists/{id}
        [HttpGet("artists/{id}")]
        public async Task<IActionResult> Get(int id) //TODO: make separate GET and POST DTOs
        {
            var foundArtist = await _artistRepository.GetSingleByConditionAsync(artist => artist.ArtistId == id, Array.Empty<string>());
            return foundArtist != null ? Ok(_mapper.Map<ArtistReadDto>(foundArtist))
                                        : NotFound();
        }

        // POST api/<MusicAPIController>/artists
        [HttpPost("artists")]
        public async Task<IActionResult> Add([FromBody] string artistName)
        {
            var savedArtist = await _artistRepository.CreateAsync(new Artist() { ArtistDescription = artistName });
            return Ok(_mapper.Map<ArtistReadDto>(savedArtist));
        }

        // PUT api/<MusicAPIController>/artists/{id}
        [HttpPut("artists/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ArtistDto artist)
        {
            artist.ArtistId = Int32.Parse(id);
            if (id != artist.ArtistId.ToString())
                return BadRequest();

            try
            {
                var updatedArtist = await _artistRepository.UpdateAsync(_mapper.Map<Artist>(artist));
                return Ok(_mapper.Map<ArtistReadDto>(updatedArtist));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE api/<MusicAPIController>/artists/{id}
        [HttpDelete("artists/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var artistToDelete = await _artistRepository.GetSingleByConditionAsync(artist => artist.ArtistId.ToString() == id, Array.Empty<string>());
                return Ok(await _artistRepository.DeleteAsync(artistToDelete));
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }
    }
}

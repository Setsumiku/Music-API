namespace Music_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPIArtistController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Artist> _artistRepository;
        private readonly LinkGenerator _linkGenerator;

        public MusicAPIArtistController(IMapper mapper, LinkGenerator linkGenerator, ILogger<MusicAPIArtistController> logger, IBaseRepository<Artist> artistRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _artistRepository = artistRepository;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Use to receive all Artists
        /// </summary>
        /// <returns>Artists</returns>
        // GET: api/<MusicAPIController>/artists
        [HttpGet("artists")]
        public async Task<IActionResult> Get()
        {
            var artists = _mapper.Map<IEnumerable<ArtistReadDto>>(await _artistRepository.GetAllAsync(Array.Empty<string>()));
            for (var index = 0; index < artists.Count(); index++)
            {
                artists.ElementAt(index).Add("Name", new { artists.ElementAt(index).ArtistDescription });
                var artistLinks = CreateLinksForArtist(artists.ElementAt(index).ArtistId);
                artists.ElementAt(index).Add("Links", artistLinks);
            }
            var artistsWrapper = new LinkCollectionWrapper<ArtistReadDto>(artists);
            return Ok(CreateLinksForArtists(artistsWrapper));
        }

        /// <summary>
        /// Use to receive specific Artist
        /// </summary>
        /// <param name="id">String for ID of Artist</param>
        /// <returns>Artist</returns>
        // GET api/<MusicAPIController>/artists/{id}
        [HttpGet("artists/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var foundArtist = await _artistRepository.GetSingleByConditionAsync(artist => artist.ArtistId == id, Array.Empty<string>());
            if (foundArtist is null) return NotFound();
            var mappedArtist = _mapper.Map<ArtistReadDto>(foundArtist);
            mappedArtist.Add("Name", new { mappedArtist.ArtistDescription });
            mappedArtist.Add("Links", CreateLinksForArtist(foundArtist.ArtistId));
            return Ok(mappedArtist);
        }

        /// <summary>
        /// Use to Create a new Artist
        /// </summary>
        /// <param name="artistName">String for Artist Description</param>
        /// <returns>Created Artist</returns>
        // POST api/<MusicAPIController>/artists
        [HttpPost("artists")]
        public async Task<IActionResult> Add([FromBody] string artistName)
        {
            try
            {
                var savedArtist = await _artistRepository.CreateAsync(new Artist() { ArtistDescription = artistName });
                return Created("api/MusicAPIArtist/artists/" + savedArtist.ArtistId, _mapper.Map<ArtistReadDto>(savedArtist));
            }
            catch (Exception ex) when (ex is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Use to Edit Artist
        /// </summary>
        /// <param name="id">String for ID of Artist</param>
        /// <param name="artistName">String for new name for the Artist</param>
        /// <returns>Updated Artist</returns>
        // PUT api/<MusicAPIController>/artists/{id}
        [HttpPut("artists/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string artistName)
        {
            try
            {
                var artistToUpdate = await _artistRepository.GetSingleByConditionAsync(artist => artist.ArtistId.ToString() == id, Array.Empty<string>());
                if (artistToUpdate != null)
                {
                    artistToUpdate.ArtistDescription = artistName;
                    _ = await _artistRepository.UpdateAsync(artistToUpdate);
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
        /// Use to Remove Artist
        /// </summary>
        /// <param name="id">String for ID of Artist</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/artists/{id}
        [HttpDelete("artists/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var artistToDelete = await _artistRepository.GetSingleByConditionAsync(artist => artist.ArtistId.ToString() == id, Array.Empty<string>());
                _ = await _artistRepository.DeleteAsync(artistToDelete);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        private IEnumerable<Link> CreateLinksForArtist(int id)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }),
                "remove_artist",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }),
                "edit_artist",
                "PUT")
            };
            return links;
        }

        private LinkCollectionWrapper<ArtistReadDto> CreateLinksForArtists(LinkCollectionWrapper<ArtistReadDto> artistsWrapper)
        {
            artistsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get)),
                    "self",
                    "GET"));
            artistsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Add)),
                "add_new_artist",
                "POST"));
            return artistsWrapper;
        }
    }
}
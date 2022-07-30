namespace Music_API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MusicAPISongController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private readonly LinkGenerator _linkGenerator;

        public MusicAPISongController(IMapper mapper, LinkGenerator linkGenerator, ILogger<MusicAPISongController> logger, IBaseRepository<Song> songRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _songRepository = songRepository;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Use to receive all Songs
        /// </summary>
        /// <returns>Songs</returns>
        // GET: api/<MusicAPIController>/songs
        [HttpGet("songs")]
        public async Task<IActionResult> Get()
        {
            var songs = _mapper.Map<IEnumerable<SongReadDto>>(await _songRepository.GetAllAsync(Array.Empty<string>()));
            for (var index = 0; index < songs.Count(); index++)
            {
                songs.ElementAt(index).Add("Name", new { songs.ElementAt(index).SongDescription });
                var songLinks = CreateLinksForSong(songs.ElementAt(index).SongId);
                songs.ElementAt(index).Add("Links", songLinks);
            }
            var songsWrapper = new LinkCollectionWrapper<SongReadDto>(songs);
            return Ok(CreateLinksForSongs(songsWrapper));
        }

        /// <summary>
        /// Use to receive specific Song
        /// </summary>
        /// <param name="id">String for ID of Song</param>
        /// <returns>Song</returns>
        // GET api/<MusicAPIController>/songs/{id}
        [HttpGet("songs/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var foundSong = await _songRepository.GetSingleByConditionAsync(song => song.SongId == id, Array.Empty<string>());
            if (foundSong is null) return NotFound();
            var mappedSong = _mapper.Map<SongReadDto>(foundSong);
            mappedSong.Add("Name", new { mappedSong.SongDescription });
            mappedSong.Add("Links", CreateLinksForSong(foundSong.SongId));
            return Ok(mappedSong);
        }

        /// <summary>
        /// Use to Create a new Song
        /// </summary>
        /// <param name="songName">String for Song Description</param>
        /// <returns>Created Song</returns>
        // POST api/<MusicAPIController>/songs
        [HttpPost("songs")]
        public async Task<IActionResult> Add([FromBody] string songName)
        {
            var savedSong = await _songRepository.CreateAsync(new Song() { SongDescription = songName });
            return Created("api/MusicAPISong/songs/" + savedSong.SongId, _mapper.Map<SongReadDto>(savedSong));
        }

        /// <summary>
        /// Use to Edit Song
        /// </summary>
        /// <param name="id">String for ID of Song</param>
        /// <param name="songName">String for new name for the Song</param>
        /// <returns>Updated Song</returns>
        // PUT api/<MusicAPIController>/songs/{id}
        [HttpPut("songs/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] string songName)
        {
            try
            {
                var songToUpdate = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id, Array.Empty<string>());
                if (songToUpdate is not null)
                {
                    songToUpdate.SongDescription = songName;
                    _ = await _songRepository.UpdateAsync(songToUpdate);
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
        /// Use to Remove Song
        /// </summary>
        /// <param name="id">String for ID of Song</param>
        /// <returns>Code for success or failure</returns>
        // DELETE api/<MusicAPIController>/songs/{id}
        [HttpDelete("songs/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var songToDelete = await _songRepository.GetSingleByConditionAsync(song => song.SongId.ToString() == id, Array.Empty<string>());
                _ = await _songRepository.DeleteAsync(songToDelete);
                return Ok();
            }
            catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException || e is DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        private IEnumerable<Link> CreateLinksForSong(int id)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get), values: new { id }),
                "self",
                "GET"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Delete), values: new { id }),
                "remove_song",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Update), values: new { id }),
                "edit_song",
                "PUT")
            };
            return links;
        }

        private LinkCollectionWrapper<SongReadDto> CreateLinksForSongs(LinkCollectionWrapper<SongReadDto> songsWrapper)
        {
            songsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Get)),
                    "self",
                    "GET"));
            songsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(Add)),
                "add_new_song",
                "POST"));
            return songsWrapper;
        }
    }
}
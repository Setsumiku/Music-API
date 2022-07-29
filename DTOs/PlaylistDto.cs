using System.ComponentModel.DataAnnotations;

namespace Music_API.DTOs
{
    public class PlaylistDto
    {
        internal int PlaylistId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        public string PlaylistDescription { get; set; }
        
        internal List<SongDto>? PlaylistSongs { get; set; }
    }
}

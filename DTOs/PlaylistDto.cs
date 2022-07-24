using Music_API.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace Music_API.DTOs
{
    public class PlaylistDto
    {
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        public string PlaylistDescription { get; set; }
        public List<Song> PlaylistSongs { get; set; }
    }
}

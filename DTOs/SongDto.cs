using Music_API.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace Music_API.DTOs
{
    public class SongDto
    {
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        public string SongDescription { get; set; }
        public Album SongAlbum { get; set; }
    }
}

using Music_API.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace Music_API.DTOs
{
    public class ArtistDto
    {
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        public string ArtistDescription { get; set; }
        public List<Album> ArtistAlbums { get; set; }
    }
}

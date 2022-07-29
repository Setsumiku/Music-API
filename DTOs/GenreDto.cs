using Music_API.Data.Model;
using System.ComponentModel.DataAnnotations;
namespace Music_API.DTOs
{
    public class GenreDto
    {
        public int GenreId { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        public string GenreDescription { get; set; }
        public List<AlbumDto>? GenreAlbums { get; set; }
    }
}
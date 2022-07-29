using Music_API.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace Music_API.DTOs
{
    public class AlbumDto
    {
        public int AlbumId { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        public string AlbumDescription { get; set; }
        public List<SongDto>? AlbumSongs { get; set; }
        public ArtistDto? Artist { get; set; }
        public Genre? Genre { get; set; }
    }
}

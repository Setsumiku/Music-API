using Music_API.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace Music_API.DTOs
{
    public class AlbumDto
    {
        internal int AlbumId { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(15, MinimumLength = 1)]
        /// <summary>
        /// Name of the Album
        /// </summary>
        public string AlbumDescription { get; set; }
        public List<Song>? AlbumSongs { get; set; }
        public Artist? Artist { get; set; }
    }
}

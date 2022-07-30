namespace Music_API.DTOs
{
    public class AlbumReadDto : Entity
    {
        internal int AlbumId { get; set; }

        /// <summary>
        /// Name of the Album
        /// </summary>
        public string AlbumDescription { get; set; }
    }
}
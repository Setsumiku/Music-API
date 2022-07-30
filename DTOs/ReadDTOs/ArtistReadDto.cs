namespace Music_API.DTOs
{
    public class ArtistReadDto : Entity
    {
        internal int ArtistId { get; set; }

        /// <summary>
        /// Name of the Artist
        /// </summary>
        public string ArtistDescription { get; set; }
    }
}
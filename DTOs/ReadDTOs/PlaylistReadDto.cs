namespace Music_API.DTOs
{
    public class PlaylistReadDto : Entity
    {
        internal int PlaylistId { get; set; }

        /// <summary>
        /// Name of the Playlist
        /// </summary>
        public string PlaylistDescription { get; set; }
    }
}
namespace Music_API.DTOs
{
    public class SongReadDto : Entity
    {
        internal int SongId { get; set; }

        /// <summary>
        /// Name of the Song
        /// </summary>
        public string SongDescription { get; set; }
    }
}
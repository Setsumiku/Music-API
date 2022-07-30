namespace Music_API.DTOs
{
    public class GenreReadDto : Entity
    {
        internal int GenreId { get; set; }

        /// <summary>
        /// Name of the Genre
        /// </summary>
        public string GenreDescription { get; set; }
    }
}
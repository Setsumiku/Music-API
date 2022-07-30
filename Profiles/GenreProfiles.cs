namespace Music_API.Profiles
{
    public class GenreProfiles : Profile
    {
        public GenreProfiles()
        {
            CreateMap<Genre, GenreReadDto>()
                .ForMember(genre => genre.GenreDescription, genreDto => genreDto.MapFrom(genre => genre.GenreDescription))
                .ForMember(genre => genre.GenreId, genreDto => genreDto.MapFrom(genre => genre.GenreId));
            CreateMap<GenreReadDto, Genre>()
                .ForMember(genreDto => genreDto.GenreDescription, genre => genre.MapFrom(genreDto => genreDto.GenreDescription));
        }
    }
}
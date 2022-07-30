using Music_API.Data.Model;

namespace Music_API.Profiles
{
    public class GenreProfiles : Profile
    {
        public GenreProfiles()
        {
            CreateMap<Genre, GenreReadDto>()
            .ForMember(genre => genre.GenreDescription, genreDto => genreDto.MapFrom(genre => genre.GenreDescription))
            .ForMember(genre => genre.GenreId, genreDto => genreDto.MapFrom(genre => genre.GenreId));
        }
    }
}

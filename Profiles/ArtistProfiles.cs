using Music_API.Data.Model;

namespace Music_API.Profiles
{
    public class ArtistProfiles : Profile
    {
        public ArtistProfiles()
        {
            CreateMap<Artist, ArtistReadDto>()
                .ForMember(artist => artist.ArtistDescription, artistDto => artistDto.MapFrom(artist => artist.ArtistDescription))
                .ForMember(artist => artist.ArtistId, artistDto => artistDto.MapFrom(artist => artist.ArtistId));
            CreateMap<ArtistReadDto, Artist>()
                .ForMember(artistDto => artistDto.ArtistDescription, artist => artist.MapFrom(artistDto => artistDto.ArtistDescription));
        }
    }
}

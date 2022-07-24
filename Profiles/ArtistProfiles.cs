using AutoMapper;
using Music_API.Data.Model;
using Music_API.DTOs;

namespace Music_API.Profiles
{
    public class ArtistProfiles : Profile
    {
        public ArtistProfiles()
        {
            CreateMap<Artist, ArtistDto>()
                .ForMember(artist=>artist.ArtistAlbums, artistDto => artistDto.MapFrom(artist=>artist.ArtistAlbums))
                .ForMember(artist=>artist.ArtistDescription, artistDto => artistDto.MapFrom(artist=>artist.ArtistDescription));
        }
    }
}

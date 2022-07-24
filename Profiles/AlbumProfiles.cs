using AutoMapper;
using Music_API.Data.Model;
using Music_API.DTOs;

namespace Music_API.Profiles
{
    public class AlbumProfiles : Profile
    {
        public AlbumProfiles()
        {
            CreateMap<Album, AlbumDto>()
                .ForMember(album => album.AlbumArtist, albumDto => albumDto.MapFrom(album => album.AlbumArtist))
                .ForMember(album => album.AlbumDescription, albumDto => albumDto.MapFrom(album => album.AlbumDescription))
                .ForMember(album => album.Songs, albumDto => albumDto.MapFrom(album => album.Songs));
        }
    }
}

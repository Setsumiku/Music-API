using Music_API.Data.Model;

namespace Music_API.Profiles
{
    public class AlbumProfiles : Profile
    {
        public AlbumProfiles()
        {
            CreateMap<Album, AlbumReadDto>()
                .ForMember(album => album.AlbumDescription, albumDto => albumDto.MapFrom(album => album.AlbumDescription))
                .ForMember(album => album.AlbumId, albumDto => albumDto.MapFrom(album => album.AlbumId));
            CreateMap<AlbumReadDto, Album>()
                .ForMember(albumDto => albumDto.AlbumDescription, album => album.MapFrom(albumDto => albumDto.AlbumDescription));
        }
    }
}
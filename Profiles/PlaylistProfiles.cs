using Music_API.Data.Model;

namespace Music_API.Profiles
{
    public class PlaylistProfiles : Profile
    {
        public PlaylistProfiles()
        {
            CreateMap<Playlist, PlaylistReadDto>()
                .ForMember(playlist => playlist.PlaylistDescription, playlistDto => playlistDto.MapFrom(playlist => playlist.PlaylistDescription))
                .ForMember(playlist => playlist.PlaylistId, playlistDto => playlistDto.MapFrom(playlist => playlist.PlaylistId));
        }
    }
}

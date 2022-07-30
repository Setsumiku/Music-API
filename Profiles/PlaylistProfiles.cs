using AutoMapper;
using Music_API.Data.Model;
using Music_API.DTOs;

namespace Music_API.Profiles
{
    public class PlaylistProfiles : Profile
    {
        public PlaylistProfiles()
        {
            CreateMap<Playlist, PlaylistDto>()
                .ForMember(playlist => playlist.PlaylistDescription, playlistDto => playlistDto.MapFrom(playlist => playlist.PlaylistDescription))
                .ForMember(playlist => playlist.PlaylistSongs, playlistDto => playlistDto.MapFrom(playlist => playlist.PlaylistSongs));
            CreateMap<Playlist, PlaylistReadDto>()
                .ForMember(playlist => playlist.PlaylistDescription, playlistDto => playlistDto.MapFrom(playlist => playlist.PlaylistDescription))
                .ForMember(playlist => playlist.PlaylistId, playlistDto => playlistDto.MapFrom(playlist => playlist.PlaylistId));
        }
    }
}

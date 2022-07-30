using AutoMapper;
using Music_API.Data.Model;
using Music_API.DTOs;

namespace Music_API.Profiles
{
    public class SongProfiles : Profile
    {
        public SongProfiles()
        {
            CreateMap<Song, SongDto>()
                .ForMember(song => song.SongDescription, songDto => songDto.MapFrom(song => song.SongDescription))
                .ForMember(song => song.Playlist, songDto => songDto.MapFrom(song => song.Playlist))
                .ForMember(song => song.Album, songDto => songDto.MapFrom(song => song.Album));
            CreateMap<Song, SongReadDto>()
                .ForMember(song => song.SongDescription, songDto => songDto.MapFrom(song => song.SongDescription))
                .ForMember(song => song.SongId, songDto => songDto.MapFrom(song => song.SongId));

        }
    }
}

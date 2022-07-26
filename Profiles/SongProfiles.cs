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
                .ForMember(song => song.SongDescription, songDto => songDto.MapFrom(song => song.SongDescription));
            CreateMap<Song, SongReadDto>()
                .ForMember(song => song.SongDescription, songDto => songDto.MapFrom(song => song.SongDescription));
        }
    }
}

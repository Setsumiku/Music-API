namespace Music_API.Profiles
{
    public class SongProfiles : Profile
    {
        public SongProfiles()
        {
            CreateMap<Song, SongReadDto>()
                .ForMember(song => song.SongDescription, songDto => songDto.MapFrom(song => song.SongDescription))
                .ForMember(song => song.SongId, songDto => songDto.MapFrom(song => song.SongId));
            CreateMap<SongReadDto, Song>()
                .ForMember(songDto => songDto.SongId, song => song.MapFrom(songDto => songDto.SongDescription));
        }
    }
}
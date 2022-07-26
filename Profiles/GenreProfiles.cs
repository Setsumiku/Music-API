﻿using AutoMapper;
using Music_API.Data.Model;
using Music_API.DTOs;

namespace Music_API.Profiles
{
    public class GenreProfiles : Profile
    {
        public GenreProfiles()
        {
            CreateMap<Genre, GenreDto>()
                .ForMember(genre => genre.GenreAlbums, genreDto => genreDto.MapFrom(genre => genre.GenreAlbums))
                .ForMember(genre => genre.GenreDescription, genreDto => genreDto.MapFrom(genre => genre.GenreDescription));
            CreateMap<Genre, GenreReadDto>()
            .ForMember(genre => genre.GenreDescription, genreDto => genreDto.MapFrom(genre => genre.GenreDescription));
        }
    }
}
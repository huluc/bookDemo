using AutoMapper;
using BookDemo.Application.DTOs;
using BookDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookForUpdateDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Book, BookForUpdateDto>();
            CreateMap<Book, BookDto>();
            CreateMap<BookForCreationDto, Book>();
            CreateMap<Book, BookDtoV2>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.BookCategories.Select(bc => bc.Category)));
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryForCreationDto, Category>();
            CreateMap<CategoryForUpdateDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}

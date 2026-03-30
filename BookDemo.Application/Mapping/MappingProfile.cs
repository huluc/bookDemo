using AutoMapper;
using BookDemo.Application.DTOs;
using Entities.Models;
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
                 .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Book, BookForUpdateDto>();
            CreateMap<Book, BookDto>();
        }
    }
}

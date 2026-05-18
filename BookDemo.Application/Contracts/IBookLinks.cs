using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface IBookLinks
    {
        LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, LinkParameters linkParameters);
    }
}

using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface IBookLinks<T>
    {
        LinkResponse TryGenerateLinks(IEnumerable<T> booksDto, LinkParameters linkParameters);
    }
}

using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts.V2
{
    /// <summary>
    /// V2 book service — returns books with Author field.
    /// </summary>
    public interface IBookService : IBookServiceBase
    {
        /// <summary>
        /// Returns all books with Author field introduced in V2.
        /// </summary>
        Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters);
    }
}

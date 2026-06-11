using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts.V1
{
    /// <summary>
    /// V1 book service — returns books without Author field.
    /// </summary>
    public interface IBookService : IBookServiceBase
    {
        /// <summary>
        /// Returns all books (read-only).
        /// Tracking is disabled internally for performance.
        /// </summary>
        Task<(LinkResponse linkResponse, MetaData MetaData)> GetBooksAsync(LinkParameters parameters);
    }
}

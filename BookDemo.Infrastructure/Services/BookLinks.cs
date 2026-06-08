using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using BookDemo.Application.RequestFeatures;
using Microsoft.AspNetCore.SignalR;
using BookDemo.Application.Constants;
using System.Dynamic;

namespace BookDemo.Infrastructure.Services
{
    public class BookLinks<T> : IBookLinks<T>
    {
        private readonly IDataShaper<T> _dataShaper;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public BookLinks(IDataShaper<T> dataShaper, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _dataShaper = dataShaper;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;

        }

        public LinkResponse TryGenerateLinks(IEnumerable<T> booksDto, LinkParameters linkParameters)
        {
            var shapedBooks = _dataShaper.ShapeData(booksDto, linkParameters.Fields);
            if (ShouldGenerateLinks())
                return ReturnLinkedBooks(shapedBooks, linkParameters);
            return ReturnShapedBooks(shapedBooks);
        }

        private LinkResponse ReturnLinkedBooks(IEnumerable<ShapedEntity> shapedBooks, LinkParameters linkParameters)
        {
            var shapedBooksList = shapedBooks.ToList();
            foreach (IDictionary<string, object> book in shapedBooksList)
            {
                var bookLinks = CreateLinksForBook(Convert.ToInt32(book["Id"]));
                book.Add("Links", bookLinks);
            }
            var bookCollection = new LinkCollectionWrapper<ShapedEntity>(shapedBooksList);
            var collectionLinks = CreateLinksForBooks();
            bookCollection.Links.AddRange(collectionLinks);


            var linkedResponse = new LinkResponse()
            {
                HasLinks = true,
                LinkedEntities = bookCollection
            };
            return linkedResponse;
        }

        private List<LinkDto> CreateLinksForBooks()
        {
            return new List<LinkDto>()
            {
                new LinkDto(href: GetUri(BookRoutes.GetAll, null), rel: "self", method: "GET"),
                new LinkDto(href: GetUri(BookRoutes.Create, null), rel: "create", method: "POST")
            };
        }

        private List<LinkDto> CreateLinksForBook(int id)
        {
            return new List<LinkDto>
            {
                new LinkDto(GetUri(BookRoutes.GetById, new { id }), "self", "GET"),
                new LinkDto(GetUri(BookRoutes.Update, new { id }), "update", "PUT"),
                new LinkDto(GetUri(BookRoutes.Delete, new { id }), "delete", "DELETE")
            };
        }

        private LinkResponse ReturnShapedBooks(IEnumerable<ShapedEntity> shapedBooks)
        {
            return new LinkResponse()
            {
                ShapedEntities = shapedBooks.ToList()
            };
        }

        private bool ShouldGenerateLinks()
        {
            var mediaType = _httpContextAccessor.HttpContext?.Items["AcceptHeaderMediaType"];
            return mediaType is MediaTypeHeaderValue parsedMediaType
       && parsedMediaType
           .SubTypeWithoutSuffix
           .EndsWith("hateoas",
               StringComparison.InvariantCultureIgnoreCase);
        }

        private HttpContext GetHttpContext() =>
            _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HTTP context is not available.");

        private string GetUri(string routeName, object? values = null) =>
            _linkGenerator.GetUriByName(GetHttpContext(), routeName, values)
                ?? throw new InvalidOperationException($"Unable to generate URI for route '{routeName}'.");
    }
}
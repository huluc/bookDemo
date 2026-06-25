using Asp.Versioning;
using BookDemo.Application.Constants;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using BookDemo.Presentation.Filters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookDemo.Presentation.Controllers
{
    [ServiceFilter(typeof(LogActionAttribute))]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/books")]
    [ApiController]
    [ResponseCache(CacheProfileName = "60SecondsDuration")]
    public class BooksV2Controller : ControllerBase
    {
        private readonly IServiceManager _services;

        public BooksV2Controller(IServiceManager services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        [HttpHead]
        [HttpGet(Name = BookRoutes.GetAllV2)]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryParameters bookQueryParameters, [FromQuery] string? fields)
        {
            // Manually construct LinkParameters to avoid [FromQuery] nested binding issue.
            // ASP.NET model binder cannot bind nested objects from query strings directly.
            var parameters = new LinkParameters(bookQueryParameters, fields);

            if (!parameters.BookQueryParameters.ValidPriceRange)
                return BadRequest("MaxPrice must be greater than or equal to MinPrice.");

            var result = await _services.BookServiceV2.GetBooksAsync(parameters);

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(result.MetaData);

            return Ok(result.linkResponse.GetResult());
        }

        [HttpGet("{id:int}", Name = BookRoutes.GetByIdV2)]
        public async Task<IActionResult> GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = await _services.BookServiceV2.GetBookByIdAsync(id);
            return Ok(book);
        }

        [HttpPost(Name = BookRoutes.CreateV2)]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookDto)
        {
            if (bookDto is null)
                return BadRequest("Book can not be null");

            var created = await _services.BookServiceV2.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}", Name = BookRoutes.UpdateV2)]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] BookForUpdateDto book)
        {
            if (book is null)
                return BadRequest("Book cannot be null");

            await _services.BookServiceV2.UpdateBookAsync(id, book);
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = BookRoutes.DeleteV2)]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            await _services.BookServiceV2.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = BookRoutes.PatchV2)]
        public async Task<IActionResult> PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<BookForUpdateDto> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest("Book patch cannot be null");

            var result = await _services.BookServiceV2.GetBookForPatchAsync(id);
            var bookToPatch = result.bookToPatch;
            var bookEntity = result.bookEntity;

            bookPatch.ApplyTo(bookToPatch, ModelState);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (!TryValidateModel(bookToPatch))
                return ValidationProblem(ModelState);

            await _services.BookServiceV2.SaveChangesForPatchAsync(bookToPatch, bookEntity);
            return NoContent();
        }

        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, DELETE, OPTIONS");
            return Ok();
        }
    }
}
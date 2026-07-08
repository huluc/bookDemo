using Asp.Versioning;
using BookDemo.Application.Constants;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Application.Models.LinkModels;
using BookDemo.Application.RequestFeatures;
using BookDemo.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookDemo.Presentation.Controllers
{
    // We separated the Presentation layer from the Web API host project
    // to clearly isolate HTTP and framework-related concerns.
    //
    // The Web API project is only responsible for starting the application
    // (Program.cs, middleware configuration, dependency injection setup).
    //
    // The Presentation layer contains controllers and everything related to
    // handling HTTP requests (routing, model binding, validation, ModelState, etc.).

    // This separation keeps the host thin and allows the same Presentation layer
    // to be reused with different hosts if needed.
    [Authorize]
    [ServiceFilter(typeof(LogActionAttribute))] // Apply validation filter to all actions in this controller
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly IServiceManager _services;

        public BooksController(IServiceManager services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        [HttpHead]
        [HttpGet(Name = BookRoutes.GetAll)]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))] // Apply media type validation filter to this action
        // Marked as virtual to allow BooksV2Controller to override
         // this method with V2-specific logic while inheriting all other actions.
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryParameters bookQueryParameters, [FromQuery] string? fields)
        {
            // Manually construct LinkParameters to avoid [FromQuery] nested binding issue.
            // ASP.NET model binder cannot bind nested objects from query strings directly.
            var parameters = new LinkParameters(bookQueryParameters, fields);
           
            // TODO: Move cross-property validation to model level via custom validation.

            if (!parameters.BookQueryParameters.ValidPriceRange)
                return BadRequest("MaxPrice must be greater than or equal to MinPrice.");

            var result = await _services
                .BookService
                .GetBooksAsync(parameters);

            Response.Headers.Append(
                   "X-Pagination",
                   System.Text.Json.JsonSerializer.Serialize(result.MetaData));

            return Ok(result.linkResponse.GetResult());

        }

        [HttpGet("{id:int}", Name = BookRoutes.GetById)]
        public async Task<IActionResult> GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = await _services.BookService.GetBookByIdAsync(id);
            return Ok(book);

        }
        [HttpPost(Name = BookRoutes.Create)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookDto)
        {
            if (bookDto is null)
                return BadRequest("Book can not be null");

            var created = await _services.BookService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);

        }
        [HttpPut("{id:int}", Name = BookRoutes.Update)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] BookForUpdateDto book)
        {

            if (book is null)
                return BadRequest("Book cannot be null");

            await _services.BookService.UpdateBookAsync(id, book);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}", Name = BookRoutes.Delete)]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            await _services.BookService.DeleteBookAsync(id);
            return NoContent(); // 204
        }


        [HttpPatch("{id:int}", Name = BookRoutes.Patch)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<BookForUpdateDto> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest("Book patch cannot be null");

            var result = await _services.BookService.GetBookForPatchAsync(id);

            var bookToPatch = result.bookToPatch;
            var bookEntity = result.bookEntity;

            // Apply patch and collect JSON Patch errors into ModelState
            bookPatch.ApplyTo(bookToPatch, ModelState);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Validate patched DTO after applying JSON Patch
            if (!TryValidateModel(bookToPatch))
                return ValidationProblem(ModelState);


            await _services.BookService.SaveChangesForPatchAsync(bookToPatch, bookEntity); // Update the entity in the data store

            return NoContent(); // 204
        }

        [AllowAnonymous]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, DELETE, OPTIONS");
            return Ok();
        }

    }
}

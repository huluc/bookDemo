using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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

    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly IServiceManager _services;

        public BooksController(IServiceManager services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _services.BookService.GetBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = await _services.BookService.GetBookByIdAsync(id);
            return Ok(book);

        }
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookDto)
        {
            if (bookDto is null)
                return BadRequest("Book can not be null");

            var created = await _services.BookService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);

        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] BookForUpdateDto book)
        {

            if (book is null)
                return BadRequest("Book cannot be null");

            await _services.BookService.UpdateBookAsync(id, book);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            await _services.BookService.DeleteBookAsync(id);
            return NoContent(); // 204
        }


        [HttpPatch("{id:int}")]
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


            await _services.BookService.SaveChangesForPathAsync(bookToPatch, bookEntity); // Update the entity in the data store

            return NoContent(); // 204
        }

    }
}

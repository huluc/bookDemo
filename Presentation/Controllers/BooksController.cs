using BookDemo.Application.Contracts;
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
        public IActionResult GetBooks()
        {
            var books = _services.BookService.GetBooks();
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = _services.BookService.GetBookById(id);
            return Ok(book);

        }
        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            if (book is null)

                return BadRequest("Book can not be null");

            var created = _services.BookService.CreateBook(book);
            return CreatedAtAction(nameof(GetBookById), new { id = created.Id }, created);

        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateBook([FromRoute] int id, [FromBody] Book book)
        {

            if (book is null)
                return BadRequest("Book cannot be null");

            if (id != book.Id)
                return BadRequest("Book ID mismatch");

            _services.BookService.UpdateBook(id, book);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var ok = _services.BookService.DeleteBook(id);
            return NoContent(); // 204
        }


        [HttpPatch("{id:int}")]
        public IActionResult PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest("Book patch cannot be null");

            var book = _services.BookService.GetBookById(id);
           
            // Apply patch and collect JSON Patch errors into ModelState
            bookPatch.ApplyTo(book, ModelState);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Validate entity after patch (DataAnnotations etc.)
            if (!TryValidateModel(book))
                return ValidationProblem(ModelState);

            _services.BookService.UpdateBook(id, book); // Update the entity in the data store

            return NoContent(); // 204
        }

    }
}

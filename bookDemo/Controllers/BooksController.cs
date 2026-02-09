using BookDemo.Application.Contracts;
using BookDemo.Infrastructure.Persistence;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace bookDemo.Controllers
{
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
            var books = _services.BookService.GetBooks() ;
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = _services.BookService.GetBookById(id);
            return book is null ? NotFound() : Ok(book);

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

            if (book is null || id != book.Id)
                return BadRequest("Book ID mismatch");

            var ok = _services.BookService.UpdateBook(id, book);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var ok =  _services.BookService.DeleteBook(id);
            return ok ? NoContent() : NotFound();


            return NoContent(); // 204
        }


        [HttpPatch("{id:int}")]
        public IActionResult PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest("Book patch cannot be null");

            var book = _services.BookService.GetBookById(id);
            if (book is null)
                return NotFound();

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

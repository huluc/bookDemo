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
        // BooksController depends on RepositoryContext.
        // The controller does not create it itself; the dependency is injected
        // from the outside by the DI container.
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _manager.Books.GetAll(false); 
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = _manager.Books.GetById(id,false);
            return book is null ? NotFound() : Ok(book);

        }
        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            if (book is null)

                return BadRequest("Book can not be null");

            _manager.Books.Add(book);
            _manager.Save(); // Commit changes to the database

            // After successfully saving the entity, this returns HTTP 201 Created and sets
            // the Location header to the URL of the GetBookById endpoint,
            // allowing clients to fetch the created resource via its identifier.
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateBook([FromRoute] int id, [FromBody] Book book)
        {
            if (id != book.Id)
                return BadRequest("Book ID mismatch");

            var existingBook = _manager.Books.GetById(id,true);
            if (existingBook is null)
                return NotFound();

            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            _manager.Save();

            return NoContent(); // 204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var book = _manager.Books.GetById(id,true);
            if (book is null)
                return NotFound();
            
            _manager.Books.Delete(book);
            _manager.Save();

            return NoContent(); // 204
        }


        [HttpPatch("{id:int}")]
        public IActionResult PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest("Book patch cannot be null");

            var book = _manager.Books.GetById(id, true);
            if (book is null)
                return NotFound();

            // Apply patch and collect JSON Patch errors into ModelState
            bookPatch.ApplyTo(book, ModelState);

            // Validate entity after patch (DataAnnotations etc.)
            if (!TryValidateModel(book))
                return ValidationProblem(ModelState);
    
            _manager.Save();
            return NoContent(); // 204
        }
    }
}

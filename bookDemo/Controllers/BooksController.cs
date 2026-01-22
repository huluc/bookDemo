
using bookDemo.Models;
using bookDemo.Repositories;
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
        private readonly RepositoryContext _context;
        public BooksController(RepositoryContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetBooks()
        {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = _context.Books.Find(id);
            return book is null ? NotFound() : Ok(book);

        }
        [HttpPost]
        public IActionResult AddBook([FromBody] Book book)
        {
            if (book is null)

                return BadRequest("Book can not be null");

            _context.Books.Add(book);
            _context.SaveChanges();

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

            var existingBook = _context.Books.Find(id);
            if (existingBook is null)
                return NotFound();

            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            _context.SaveChanges();

            return NoContent(); // 204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var book = _context.Books.Find(id);
            if (book is null)
                return NotFound();
            
            _context.Books.Remove(book);
            _context.SaveChanges();

            return NoContent(); // 204
        }


        [HttpPatch("{id:int}")]
        public IActionResult PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest("Book patch cannot be null");

            var book = _context.Books.Find(id);
            if (book is null)
                return NotFound();

            // Apply patch and collect JSON Patch errors into ModelState
            bookPatch.ApplyTo(book, ModelState);

            // Validate entity after patch (DataAnnotations etc.)
            if (!TryValidateModel(book))
                return ValidationProblem(ModelState);
    
            _context.SaveChanges();
            return NoContent(); // 204
        }
    }
}

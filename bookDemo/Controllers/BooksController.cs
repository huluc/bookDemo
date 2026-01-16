using bookDemo.Data;
using bookDemo.Models;
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
        [HttpGet]
        public IActionResult GetBooks()
        {
            return Ok(ApplicationContext.Books);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = ApplicationContext.Books.FirstOrDefault(b => b.Id == id);
            return book is null ? NotFound() : Ok(book);

        }
        [HttpPost]
        public IActionResult AddBook([FromBody] Book book)
        {
            if (book is null)

                return BadRequest("Book can not be null");

            book.Id = ApplicationContext.Books.Any()
                ? ApplicationContext.Books.Max(b => b.Id) + 1
                : 1;
            ApplicationContext.Books.Add(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateBook([FromRoute] int id, [FromBody] Book book)
        {
            if (book is null)
                return BadRequest("Book cannot be null");

            if (id != book.Id)
                return BadRequest("Book ID mismatch");

            var existing = ApplicationContext.Books.FirstOrDefault(b => b.Id == id);
            if (existing is null)
                return NotFound();

            existing.Title = book.Title;
            existing.Price = book.Price;

            return NoContent(); // 204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var book = ApplicationContext.Books.FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound(new
                {
                    Message = $"Book with ID {id} not found."
                });
            ApplicationContext.Books.Remove(book);
            return NoContent(); // 204
        }
        [HttpDelete]
        public IActionResult DeleteAllBooks()
        {
            ApplicationContext.Books.Clear();
            return NoContent(); // 204
        }

        [HttpPatch("{id:int}")]
        public IActionResult PatchBook([FromRoute] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var book = ApplicationContext.Books.FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

         bookPatch.ApplyTo(book, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return NoContent(); // 204
        }
    }
}

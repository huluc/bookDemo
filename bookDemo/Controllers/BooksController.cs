using bookDemo.Data;
using bookDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
            return book is null ? NotFound() : Ok();

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
    }
}

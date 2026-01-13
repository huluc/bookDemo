using bookDemo.Data;
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
            return Ok(Data.ApplicationContext.Books);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = Data.ApplicationContext.Books.FirstOrDefault(b => b.Id == id);
            return book is null ? NotFound() : Ok();

        }
    }
}

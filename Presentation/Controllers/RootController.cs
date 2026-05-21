using BookDemo.Application.Constants;
using BookDemo.Application.Models.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BookDemo.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        private const string BooksRel = "books";

        public RootController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HttpGet(Name = "GetRoot")]
        public async Task<IActionResult> GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType.Contains(MediaTypes.ApiRoot))
            {
                var list = new List<LinkDto>
                {
                    new LinkDto
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), null)
                        ?? string.Empty,
                        Rel = "self",
                        Method = "GET"
                    },
                    new LinkDto
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, nameof(BooksController.GetBooks), null)
                        ?? string.Empty,
                        Rel = BooksRel,
                        Method = "GET"
                    },
                    new LinkDto
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, nameof(BooksController.CreateBook), null)
                        ?? string.Empty,
                        Rel = BooksRel,
                        Method = "POST"
                    }
                };
                return Ok(list);
            }
            return NoContent();
        }
    }
}

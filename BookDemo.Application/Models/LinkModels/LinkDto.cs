using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Models.LinkModels
{
    public class LinkDto
    {
        public string? Href { get; set; }
        public string? Rel { get; set; }
        public string? Method { get; set; }

        public LinkDto()
        {

        }

        public LinkDto(string? href, string? rel, string? method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
    public class LinkResourceBaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}

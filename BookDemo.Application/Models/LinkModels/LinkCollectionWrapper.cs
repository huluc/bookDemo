using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Models.LinkModels
{
    public class LinkCollectionWrapper<T> : LinkResourceBaseDto
    {
        public List<T> Value { get; set; } = new List<T>();
        public LinkCollectionWrapper()
        {
            
        }
        public LinkCollectionWrapper(List<T> value)
        {
            Value = value;
        }
    }
}

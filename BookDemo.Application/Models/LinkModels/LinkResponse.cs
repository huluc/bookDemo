using BookDemo.Application.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Models.LinkModels
{
    public class LinkResponse
    {
        public bool HasLinks { get; set; }

        public List<ShapedEntity>? ShapedEntities { get; set; }

        public LinkCollectionWrapper<ShapedEntity>? LinkedEntities { get; set; }

        public object GetResult() => HasLinks ? LinkedEntities! : ShapedEntities!;

    }
}

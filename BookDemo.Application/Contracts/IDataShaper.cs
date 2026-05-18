using BookDemo.Application.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);

        ShapedEntity ShapeData(T entity, string fieldsString);


    }
}

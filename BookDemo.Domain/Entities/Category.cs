using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }


        // Navigation property to the join entity — lets EF Core traverse
        // Category -> BookCategory -> Book without needing a direct Category-Book link.
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}

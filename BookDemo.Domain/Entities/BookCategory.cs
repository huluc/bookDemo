namespace BookDemo.Domain.Entities
{
    // Explicit join entity for the Book <-> Category many-to-many relationship.
    // Kept explicit (rather than EF Core's implicit skip-navigation) so the
    // relationship can carry extra data later if needed (e.g. AddedDate),
    // and so the composite key / FKs are visible and controlled directly.
    public class BookCategory
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

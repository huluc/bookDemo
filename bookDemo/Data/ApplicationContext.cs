using bookDemo.Models;

namespace bookDemo.Data
{
    public static class ApplicationContext
    {
        public static List<Book> Books { get; set; } 
        static ApplicationContext()
        {
            Books = new List<Book>()
            {
                new Book() { Id = 1, Title = "The Great Gatsby",Price =50},
                new Book() { Id = 2, Title = "To Kill a Mockingbird", Price=100 },
                new Book() { Id = 3, Title = "1984", Price=20},
                new Book() { Id = 4, Title = "Pride and Prejudice",Price=30},
                new Book() { Id = 5, Title = "The Catcher in the Rye", Price=489 }
            };
        }
    }
}

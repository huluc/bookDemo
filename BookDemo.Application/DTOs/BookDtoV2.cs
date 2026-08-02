namespace BookDemo.Application.DTOs
{
    public record BookDtoV2
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string? Author { get; init; }
        public List<CategoryDto> Categories { get; init; } = new List<CategoryDto>();
    }
}
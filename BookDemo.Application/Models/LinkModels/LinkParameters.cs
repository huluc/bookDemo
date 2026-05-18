using BookDemo.Application.RequestFeatures;

namespace BookDemo.Application.Models.LinkModels
{
    /// <summary>Fields is kept separately because it affects the response representation
    /// and must be preserved when generating HATEOAS links (data shaping support).</summary>
    /// <param name="BookQueryParameters">The query parameters for the book.</param>
    /// <param name="Fields">The fields to include in the response.</param>
    public record LinkParameters(BookQueryParameters BookQueryParameters, string? Fields)
    {
        public BookQueryParameters BookQueryParameters { get; init; } =
              BookQueryParameters ?? new BookQueryParameters();
        public string? Fields { get; init; } = string.IsNullOrWhiteSpace(Fields) ? null : Fields;
    }

}

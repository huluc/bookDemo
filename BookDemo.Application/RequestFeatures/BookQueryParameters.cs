using System.ComponentModel.DataAnnotations;

namespace BookDemo.Application.RequestFeatures
{
    public class BookQueryParameters : RequestParameters
    {
        [Range(0, double.MaxValue)]
        public decimal? MinPrice { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? MaxPrice { get; set; }
        public bool ValidPriceRange
        {
            get
            {
                if (MinPrice.HasValue && MaxPrice.HasValue)
                    return MaxPrice >= MinPrice;

                return true;
            }
        }
        // TODO: Refactor this validation into a custom ValidationAttribute (cross-property validation)
        // after initial implementation. This will move validation responsibility to the model level
        // and keep controllers/services cleaner.

        public string? SearchTerm { get; set; }
    }
}

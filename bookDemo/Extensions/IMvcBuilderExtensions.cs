using BookDemo.Presentation.Formatters;
namespace BookDemo.API.Extensions
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>
        
             builder.AddMvcOptions(options =>
            {
                options.OutputFormatters.Add(new CsvOutputFormatter());
            });
        
    }
}

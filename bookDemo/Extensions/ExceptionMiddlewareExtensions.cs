using BookDemo.Domain.Exceptions;
using BookDemo.Presentation.Models.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace BookDemo.API.Extensions
{
    // TODO:: Custom ExceptionMiddleware try/catch kullan
    /// <summary>
    /// Provides a centralized global exception handling mechanism.
    /// This extension keeps Program.cs clean and ensures consistent
    /// error responses across the entire application.
    /// </summary>
    /// middleware yazmak yerine 
    /// ASP.NET Core’un built-in (hazır) exception handler middleware’ini kullandim
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Registers the global exception handler middleware in the HTTP pipeline.
        /// 
        /// Responsibilities:
        /// ✔ Catch all unhandled exceptions
        /// ✔ Log the exception details
        /// ✔ Map known exception types to proper HTTP status codes
        /// ✔ Return a standardized JSON error response
        /// 
        /// This method should be called early in the pipeline.
        /// </summary>
        public static void UseGlobalExceptionHandling(this WebApplication app)
        {
            //ASP.NET Core’un hazır global exception yakalayıcısını pipeline’a ekler.
            app.UseExceptionHandler(errorApp =>
            {
                //hata endpoint’i tanımlar.
                //Yani bir yerde exception fırlayınca ASP.NET Core request’i buraya yönlendirir.
                //Run ASP.NET Core middleware pipeline’ında son middleware’dir.
                //Bu noktaya gelen request artık başka middleware’e gitmez. Burada response oluşturulur.
                errorApp.Run(async context =>
                {
                    //Bir exception olursa, o exception’a ait request context’ini bana ver.
                    //Ben response’u 500 ve JSON olarak ayarlayayım.
                    //Sonra da o request’e bağlı hata bilgisini features içinden okuyayım
                    var logger = context.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("GlobalExceptionHandler");

                    context.Response.ContentType = "application/json";
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    var exception = exceptionFeature?.Error;
                    if (exception is null)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        logger.LogError("An unknown error occurred without an exception. TraceId: {TraceId}", context.TraceIdentifier);

                        await context.Response.WriteAsJsonAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Unexpected server error.",
                            TraceId = context.TraceIdentifier
                        });
                        return;
                    }
                    //Ben built-in exception handler kullanıyorum. errorApp.Run(...) içindeyim.
                    //Bu bir custom middleware class’ı değil.
                    //O yüzden burada constructor injection doğal olarak kullanılamaz.
                    //Bu yapıda logger gibi servisleri context.RequestServices üzerinden almak daha uygundur.



                  
                    var (statusCode, message) = MapException(exception);

                    context.Response.StatusCode = statusCode;

                    logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);

                    await context.Response.WriteAsJsonAsync(new ErrorDetails
                    {
                        StatusCode = statusCode,
                        Message = message,
                        TraceId = context.TraceIdentifier
                    });



                });
            });
        }

        private static (int StatusCode, string message) MapException(Exception exception) =>
            exception switch
            {
                NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
                BadRequestException ex=> (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Unexpected server error.")
            };
    }
}

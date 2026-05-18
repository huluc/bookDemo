using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace BookDemo.Presentation.Filters
{
    /// <summary>
    /// Action filter that validates the Accept header before the action executes.
    ///
    /// This filter acts as a gatekeeper for media type negotiation:
    /// 1. Ensures the client has sent an Accept header
    /// 2. Ensures the Accept header contains a valid, parseable media type
    /// 3. Stores the parsed media type in HttpContext.Items so downstream
    ///    services (e.g. BookLinks) can decide whether to generate HATEOAS links
    ///
    /// Must be registered via ConfigureActionFilters() in ServiceExtensions.cs
    /// </summary>
    public class ValidateMediaTypeAttribute : ActionFilterAttribute
    {
        //It should be registered to ConfigureActionFilters() method in ServiceExtensions.cs
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Step 1: Check if the Accept header exists at all
            var acceptHeaderPresent = context.HttpContext
            .Request
            .Headers
            .ContainsKey("Accept");

            if (!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult($"Accept header is missing!");
                return;
            }

            // Step 2: Try to parse the Accept header value into a MediaTypeHeaderValue
            // Example valid value: application/vnd.hilal.bookdemo.hateoas+json
            // If parsing fails, the media type is malformed
            var mediaType = context.HttpContext
                .Request
                .Headers["Accept"]
                .ToString();

            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType))
            {
                context.Result = new BadRequestObjectResult($"Media type not present: {mediaType}"
                    + $" Please add accept header with required media type.");
                return;
            }
            // Step 3: Store the parsed media type in HttpContext.Items
            // BookLinks.ShouldGenerateLinks() reads this value to determine
            // whether to generate HATEOAS links or return plain shaped entities
            context.HttpContext.Items["AcceptHeaderMediaType"] = outMediaType;

            // TODO: Validate against the list of supported media types.
            // Without this check, unsupported media types will return 200 OK with an empty body,
            // which is confusing for clients. Returning 406 Not Acceptable explicitly
            // tells the client to retry with a supported media type.
        }
    }
}

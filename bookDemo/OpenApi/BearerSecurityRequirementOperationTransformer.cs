using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BookDemo.API.OpenApi
{
    /// <summary>
    /// Applies the Bearer security requirement only to endpoints decorated with
    /// [Authorize], skipping endpoints marked [AllowAnonymous] (e.g. login, register).
    /// </summary>
    public sealed class BearerSecurityRequirementOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            var hasAuthorize = metadata.OfType<AuthorizeAttribute>().Any();
            var hasAllowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();

            if (!hasAuthorize || hasAllowAnonymous)
            {
                return Task.CompletedTask;
            }

            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            });

            return Task.CompletedTask;
        }
    }
}

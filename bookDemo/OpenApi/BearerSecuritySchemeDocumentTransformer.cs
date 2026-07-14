using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BookDemo.API.OpenApi
{
    /// <summary>
    /// Registers the Bearer JWT security scheme definition in the OpenAPI document's
    /// Components section. Does not apply it to any endpoint — see
    /// <see cref="BearerSecurityRequirementOperationTransformer"/> for that.
    /// </summary>
    public sealed class BearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. The 'Bearer' prefix is added automatically."
            };

            document.Components ??= new OpenApiComponents();
            document.AddComponent("Bearer", bearerScheme);

            return Task.CompletedTask;
        }
    }
}

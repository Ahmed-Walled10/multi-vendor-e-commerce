using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECommerce.Api.Infrastructure.Swagger;

public class OrderTagsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // explicitly defining the tags here forces Swagger UI to render them in this exact order
        swaggerDoc.Tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "Authentication" },
            new OpenApiTag { Name = "Catalog" },
            new OpenApiTag { Name = "Variants" },
            new OpenApiTag { Name = "Attributes" }
        };
    }
}

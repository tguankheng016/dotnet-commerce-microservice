using Asp.Versioning.Builder;
using CommerceMicro.Modules.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.ProductService.Application.Products;

public class ProductConfigs
{
    public const string ApiVersionSet = "Product";
}

public static class ProductApiVersionSets
{
    public static ApiVersionSet GetApiVersionSet(this IEndpointRouteBuilder builder)
    {
        return builder.NewApiVersionSet(ProductConfigs.ApiVersionSet)
            .ToLatestApiVersion()
            .Build();
    }
}

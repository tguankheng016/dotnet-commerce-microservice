using Asp.Versioning.Builder;
using CommerceMicro.Modules.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;


namespace CommerceMicro.CartService.Application.Carts;

public class CartConfigs
{
    public const string ApiVersionSet = "Cart";
}

public static class CartApiVersionSets
{
    public static ApiVersionSet GetApiVersionSet(this IEndpointRouteBuilder builder)
    {
        return builder.NewApiVersionSet(CartConfigs.ApiVersionSet)
            .ToLatestApiVersion()
            .Build();
    }
}
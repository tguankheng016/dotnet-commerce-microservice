using Asp.Versioning.Builder;
using CommerceMicro.Modules.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.ProductService.Application.Categories;

public class CategoryConfigs
{
	public const string ApiVersionSet = "Category";
}

public static class CategoryApiVersionSets
{
	public static ApiVersionSet GetApiVersionSet(this IEndpointRouteBuilder builder)
	{
		return builder.NewApiVersionSet(CategoryConfigs.ApiVersionSet)
			.ToLatestApiVersion()
			.Build();
	}
}


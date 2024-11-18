using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using CommerceMicro.Modules.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.IdentityService.Application.Users;

public class UserConfigs
{
	public const string ApiVersionSet = "User";
}

public static class UserApiVersionSets
{
	public static ApiVersionSet GetApiVersionSet(this IEndpointRouteBuilder builder)
	{
		return builder.NewApiVersionSet(UserConfigs.ApiVersionSet)
			.ToLatestApiVersion()
			.Build();
	}
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Permissions;

public static class PermissionExtensions
{
	public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
	{
		var appPermissions = new AppPermissions()
		{
			Items = AppPermissionProvider.GetPermissions()
		};

		services.AddSingleton(appPermissions);

		services.AddScoped<IPermissionManager, PermissionManager>();
		services.AddScoped<IPermissionDbManager, DefaultPermissionDbManager>();

		// Add authorization policies
		services.AddAuthorization(options =>
		{
			foreach (var item in appPermissions.Items)
			{
				options.AddPolicy(item.Name, policy => policy.RequireClaim(PermissionConsts.PermissionClaimName, item.Name));
			}
		});

		return services;
	}

	public static IApplicationBuilder UsePermissionMiddleware(this WebApplication app)
	{
		app.UseMiddleware<PermissionMiddleware>();
		return app;
	}
}

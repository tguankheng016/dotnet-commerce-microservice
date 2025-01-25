using CommerceMicro.IdentityService.Application.Identities.Services;
using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Permissions;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.IdentityService.Application.Startup;

public static class PermissionHandlerExtension
{
	public static IServiceCollection AddCustomIdentityPermissionAuthorization(this IServiceCollection services)
	{
		services.AddPermissionAuthorization();
		services.ReplaceService<IPermissionDbManager, PermissionDbManager>();

		return services;
	}
}

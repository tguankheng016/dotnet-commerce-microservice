using CommerceMicro.IdentityService.Application.Identities.Services;
using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.IdentityService.Application.Startup;

public static class JwtTokenHandlerExtension
{
    public static IServiceCollection AddCustomIdentityJwtTokenHandler(this IServiceCollection services)
    {
        services.AddCustomJwtTokenHandler();
        services.ReplaceService<IPermissionDbManager, PermissionDbManager>();

        return services;
    }
}

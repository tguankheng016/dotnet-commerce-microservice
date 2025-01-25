using CommerceMicro.IdentityService.Application.Identities.Services;
using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.IdentityService.Application.Startup;

public static class JwtTokenHandlerExtension
{
    public static IServiceCollection AddCustomIdentityJwtTokenHandler(this IServiceCollection services)
    {
        services.AddCustomJwtTokenHandler();
        services.ReplaceService<ITokenKeyDbValidator, TokenKeyDbValidator>();
        services.ReplaceService<ITokenSecurityStampDbValidator, TokenSecurityStampDbValidator>();

        return services;
    }
}

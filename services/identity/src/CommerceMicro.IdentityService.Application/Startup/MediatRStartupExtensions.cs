using CommerceMicro.Modules.Logging;
using CommerceMicro.Modules.Core.EFCore;
using CommerceMicro.Modules.Core.Validations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.IdentityService.Application.Startup;

public static class MediatRStartupExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRStartupExtensions).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EFTransactionBehavior<,>));

        return services;
    }
}

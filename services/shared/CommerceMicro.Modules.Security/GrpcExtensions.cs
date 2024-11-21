using CommerceMicro.IdentityService;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Security;

public static class GrpcExtensions
{
    public static IServiceCollection AddIdentityGrpcClient(this IServiceCollection services, string address)
    {
        services.AddGrpcClient<IdentityGrpcService.IdentityGrpcServiceClient>(o =>
        {
            o.Address = new Uri(address);
        });

        return services;
    }
}

using CommerceMicro.PermissionService;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Permissions;

public static class GrpcExtensions
{
    public static IServiceCollection AddPermissionGrpcClient(this IServiceCollection services, string address)
    {
        services.AddGrpcClient<PermissionGrpcService.PermissionGrpcServiceClient>(o =>
        {
            o.Address = new Uri(address);
        });

        return services;
    }
}

using CommerceMicro.Modules.Core.Grpc;
using CommerceMicro.PermissionService;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Permissions;

public static class GrpcExtensions
{
	public static IServiceCollection AddPermissionGrpcClient(this IServiceCollection services, string address)
	{
		services.AddCustomGrpcClient<PermissionGrpcService.PermissionGrpcServiceClient>(address);

		return services;
	}
}

using CommerceMicro.IdentityService;
using CommerceMicro.Modules.Core.Grpc;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Security;

public static class GrpcExtensions
{
	public static IServiceCollection AddIdentityGrpcClient(this IServiceCollection services, string address)
	{
		services.AddCustomGrpcClient<IdentityGrpcService.IdentityGrpcServiceClient>(address);

		return services;
	}
}

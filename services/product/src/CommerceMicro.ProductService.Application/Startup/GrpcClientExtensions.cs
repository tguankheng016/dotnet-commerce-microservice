using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.ProductService.Application.Startup;

public static class GrpcClientExtensions
{
	public static IServiceCollection AddGrpcClients(this IServiceCollection services)
	{
		var grpcOptions = services.GetOptions<GrpcOptions>(nameof(GrpcOptions));

		services.AddIdentityGrpcClient(grpcOptions.IdentityAddress);
		services.AddPermissionGrpcClient(grpcOptions.IdentityAddress);

		return services;
	}
}

public class GrpcOptions
{
	public string IdentityAddress { get; set; } = "";
}
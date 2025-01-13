using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Security;
using CommerceMicro.ProductService;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.CartService.Application.Startup;

public static class GrpcClientExtensions
{
	public static IServiceCollection AddGrpcClients(this IServiceCollection services)
	{
		var grpcOptions = services.GetOptions<GrpcOptions>(nameof(GrpcOptions));

		services.AddGrpcClient<ProductGrpcService.ProductGrpcServiceClient>(o =>
		{
			o.Address = new Uri(grpcOptions.ProductAddress);
		});

		services.AddIdentityGrpcClient(grpcOptions.IdentityAddress);
		services.AddPermissionGrpcClient(grpcOptions.IdentityAddress);

		return services;
	}
}

public class GrpcOptions
{
	public string IdentityAddress { get; set; } = "";
	public string ProductAddress { get; set; } = "";
}
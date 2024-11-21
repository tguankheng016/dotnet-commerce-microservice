using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Core.Grpc;

public static class GrpcExtensions
{
	public static IServiceCollection AddCustomGrpcClient<TClient>(this IServiceCollection services, string address)
		where TClient : class
	{
		var defaultMethodConfig = new MethodConfig
		{
			Names = { MethodName.Default },
			RetryPolicy = new RetryPolicy
			{
				MaxAttempts = 5,
				InitialBackoff = TimeSpan.FromSeconds(1),
				MaxBackoff = TimeSpan.FromSeconds(5),
				BackoffMultiplier = 1.5,
				RetryableStatusCodes = { StatusCode.Unavailable }
			}
		};

		services.AddGrpcClient<TClient>(o =>
		{
			o.Address = new Uri(address);
			o.ChannelOptionsActions.Add(options =>
			{
				options.ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } };
			});
		});

		return services;
	}
}

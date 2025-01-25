using CommerceMicro.IdentityService.Application.Identities.Http;
using CommerceMicro.Modules.Core;
using CommerceMicro.Modules.Resiliency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.IdentityService.Application.Startup;

public static class HttpClientExtensions
{
	public static IServiceCollection AddCustomHttpClients(this IServiceCollection services, ConfigurationManager configuration)
	{
		services.AddHttpClient(OAuthApiClient.ClientName, (serviceProvider, client) =>
		{
			var baseUrl = configuration["Authentication:OpenIddict:BaseUrl"]!;

			client.DefaultRequestHeaders.Add("User-Agent", nameof(AppOptions));
			client.BaseAddress = new Uri(baseUrl);
		})
		.AddHttpClientRetryPolicyHandler()
		.AddHttpClientCircuitBreakerPolicyHandler();

		return services;
	}
}

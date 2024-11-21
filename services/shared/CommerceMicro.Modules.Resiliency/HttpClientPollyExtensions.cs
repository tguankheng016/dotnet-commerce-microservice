using CommerceMicro.Modules.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace CommerceMicro.Modules.Resiliency;

public static class HttpClientPollyExtensions
{
	public static IHttpClientBuilder AddHttpClientRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder)
	{
		return httpClientBuilder.AddPolicyHandler((sp, _) =>
		{
			var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger("PollyHttpClientRetryPoliciesLogger");

			return HttpPolicyExtensions.HandleTransientHttpError()
				.WaitAndRetryAsync(
					retryCount: options.Retry!.RetryCount,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(options.Retry.SleepDuration),
					onRetry: (response, timeSpan, retryCount, context) =>
					{
						if (response?.Exception != null)
						{
							logger.LogError(response.Exception,
								"Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.",
								response.Result?.StatusCode,
								timeSpan,
								retryCount);
						}
					}
				);
		});
	}

	public static IHttpClientBuilder AddHttpClientCircuitBreakerPolicyHandler(this IHttpClientBuilder httpClientBuilder)
	{
		return httpClientBuilder.AddPolicyHandler((sp, _) =>
		{
			var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger("PollyHttpClientCircuitBreakerPoliciesLogger");

			return HttpPolicyExtensions.HandleTransientHttpError()
				.CircuitBreakerAsync(
					handledEventsAllowedBeforeBreaking: options.CircuitBreaker!.RetryCount,
					durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreaker.BreakDuration),
					onBreak: (response, breakDuration) =>
					{
						if (response?.Exception != null)
						{
							logger.LogError(
								response.Exception,
								"Service shutdown for {BreakDuration} seconds after {RetryCount} failed retries",
								breakDuration,
								options.CircuitBreaker.RetryCount
							);
						}
					},
					onReset: () =>
					{
						logger.LogInformation("Service restarted");
					}
				);
		});
	}
}

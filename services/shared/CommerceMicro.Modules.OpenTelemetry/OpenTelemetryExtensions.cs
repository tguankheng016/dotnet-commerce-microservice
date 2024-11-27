using CommerceMicro.Modules.Core;
using CommerceMicro.Modules.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CommerceMicro.Modules.OpenTelemetry;

public static class OpenTelemetryExtensions
{
	public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services)
	{
		var appOptions = services.GetOptions<AppOptions>(nameof(AppOptions));
		var jaegerOptions = services.GetOptions<JaegerOptions>(nameof(JaegerOptions));

		services.AddOpenTelemetry()
			.ConfigureResource(resource => resource.AddService(appOptions.Name!))
			.WithTracing(tracing =>
			{
				tracing
					.AddGrpcClientInstrumentation()
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					.AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
					.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

				tracing.AddOtlpExporter(options =>
				{
					options.Endpoint = new Uri($"{jaegerOptions.HostName!}/v1/traces");
				});
			});

		return services;
	}
}

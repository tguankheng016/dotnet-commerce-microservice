using System.Reflection;
using CommerceMicro.Modules.Core.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.MassTransit;

public static class MassTransitExtensions
{
	public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IWebHostEnvironment env, Assembly assembly)
	{
		services.AddValidateOptions<RabbitMqOptions>();

		services.AddMassTransit(configure => { SetupMasstransitConfigurations(services, configure, assembly); });

		return services;
	}

	private static void SetupMasstransitConfigurations(IServiceCollection services, IBusRegistrationConfigurator configure, Assembly assembly)
	{
		configure.SetKebabCaseEndpointNameFormatter();
		configure.AddConsumers(assembly);
		// configure.AddSagaStateMachines(assembly);
		// configure.AddSagas(assembly);
		// configure.AddActivities(assembly);

		configure.UsingRabbitMq((context, configurator) =>
		{
			var rabbitMqOptions = services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));

			configurator.Host(rabbitMqOptions?.HostName, rabbitMqOptions?.Port ?? 5672, "/", h =>
			{
				h.Username(rabbitMqOptions?.UserName!);
				h.Password(rabbitMqOptions?.Password!);
			});

			configurator.ConfigureEndpoints(context);

			//configurator.UseMessageRetry(AddRetryConfiguration);
		});
	}
}

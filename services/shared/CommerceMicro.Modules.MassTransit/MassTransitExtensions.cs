using System.Reflection;
using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Core.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.MassTransit;

public static class MassTransitExtensions
{
	public static IServiceCollection AddCustomMassTransit<TContext>(this IServiceCollection services, Assembly assembly)
		where TContext : DbContext
	{
		services.AddValidateOptions<RabbitMqOptions>();

		services.AddMassTransit(configure => { SetupMasstransitConfigurations<TContext>(services, configure, assembly); });

		return services;
	}

	private static void SetupMasstransitConfigurations<TContext>(IServiceCollection services, IBusRegistrationConfigurator configure, Assembly assembly)
		where TContext : DbContext
	{
		configure.SetKebabCaseEndpointNameFormatter();
		configure.AddConsumers(assembly);

		configure.AddEntityFrameworkOutbox<TContext>(o =>
		{
			o.UsePostgres();
			o.UseBusOutbox();
		});

		configure.UsingRabbitMq((context, configurator) =>
		{
			var rabbitMqOptions = services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));

			configurator.Host(rabbitMqOptions?.HostName, rabbitMqOptions?.Port ?? 5672, "/", h =>
			{
				h.Username(rabbitMqOptions?.UserName!);
				h.Password(rabbitMqOptions?.Password!);
			});

			configurator.ConfigureEndpoints(context);

			configurator.UseMessageRetry(AddRetryConfiguration);
		});
	}

	private static void AddRetryConfiguration(IRetryConfigurator retryConfigurator)
	{
		retryConfigurator.Exponential(
			3,
			TimeSpan.FromMilliseconds(200),
			TimeSpan.FromMinutes(120),
			TimeSpan.FromMilliseconds(200)
		)
		.Ignore<ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit
	}
}

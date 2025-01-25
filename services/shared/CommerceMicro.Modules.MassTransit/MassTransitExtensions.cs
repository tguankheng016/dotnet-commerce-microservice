using System.Reflection;
using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Core.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CommerceMicro.Modules.MassTransit;

public static class MassTransitExtensions
{
	public static IServiceCollection AddCustomMassTransit<TContext>(this IServiceCollection services, Assembly assembly)
		where TContext : DbContext
	{
		services.AddValidateOptions<RabbitMqOptions>();

		services.AddMassTransit(configure =>
		{
			SetupMasstransitConfigurations(services, configure, assembly);
			SetupEntityFrameworkOutboxConfigurations<TContext>(configure);
		});

		return services;
	}

	public static IServiceCollection AddCustomMongoMassTransit(this IServiceCollection services, Assembly assembly)
	{
		services.AddValidateOptions<RabbitMqOptions>();

		services.AddMassTransit(configure =>
		{
			SetupMasstransitConfigurations(services, configure, assembly);
			//SetupMongoOutboxConfiguration(configure);
		});

		return services;
	}

	private static void SetupEntityFrameworkOutboxConfigurations<TContext>(IBusRegistrationConfigurator configure)
		where TContext : DbContext
	{
		configure.AddEntityFrameworkOutbox<TContext>(o =>
		{
			o.QueryDelay = TimeSpan.FromSeconds(1);
			o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
			o.UsePostgres();
			o.UseBusOutbox();
		});

		configure.AddConfigureEndpointsCallback((context, name, cfg) =>
		{
			cfg.UseEntityFrameworkOutbox<TContext>(context);
		});
	}

	private static void SetupMongoOutboxConfiguration(IBusRegistrationConfigurator configure)
	{
		configure.AddMongoDbOutbox(o =>
		{
			o.QueryDelay = TimeSpan.FromSeconds(1);
			o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
			o.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
			o.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());
			o.UseBusOutbox();
		});

		configure.AddConfigureEndpointsCallback((context, name, cfg) =>
		{
			cfg.UseMongoDbOutbox(context);
		});
	}

	private static void SetupMasstransitConfigurations(IServiceCollection services, IBusRegistrationConfigurator configure, Assembly assembly)
	{
		configure.SetKebabCaseEndpointNameFormatter();
		configure.AddConsumers(assembly);

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

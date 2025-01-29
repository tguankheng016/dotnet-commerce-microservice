using CommerceMicro.IdentityService.Api;
using CommerceMicro.IdentityService.Application.Startup;
using CommerceMicro.Modules.Caching;
using CommerceMicro.Modules.Postgres;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace CommerceMicro.IdentityService.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<IApplicationRoot>, IAsyncLifetime
{
	public ITestOutputHelper? Output { get; set; }

	public readonly PostgreSqlContainer DatabaseContainer = new PostgreSqlBuilder()
		.WithUsername("workshop")
		.WithPassword("password")
		.WithDatabase("mydb")
		.Build();

	public async Task InitializeAsync()
	{
		await DatabaseContainer.StartAsync();
	}

	public async new Task DisposeAsync()
	{
		await DatabaseContainer.StopAsync();
	}

	protected override IHost CreateHost(IHostBuilder builder)
	{
		builder.UseSerilog(
			(ctx, loggerConfiguration) =>
			{
				if (Output is not null)
				{
					loggerConfiguration.WriteTo.TestOutput(
						Output,
						LogEventLevel.Error,
						"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}"
					);
				}
			}
		);

		return base.CreateHost(builder);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureTestServices(services =>
		{
			var descriptor = services
				.SingleOrDefault(s => s.ServiceType == typeof(PostgresOptions));

			if (descriptor is not null)
			{
				services.Remove(descriptor);

				var newPostgresOptions = new PostgresOptions()
				{
					ConnectionString = DatabaseContainer.GetConnectionString()
				};

				services.AddSingleton(newPostgresOptions);
			}

			descriptor = services
				.SingleOrDefault(s => s.ServiceType == typeof(RedisOptions));

			if (descriptor is not null)
			{
				services.Remove(descriptor);

				var newRedisOptions = new RedisOptions()
				{
					Enabled = false
				};

				services.AddSingleton(newRedisOptions);
			}

			services.AddMassTransitTestHarness(configure =>
			{
				configure.AddConsumers(typeof(InfrastructureExtensions).Assembly);
				configure.SetTestTimeouts(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
				configure.UsingInMemory((context, cfg) =>
				{
					cfg.ConfigureEndpoints(context);
				});
			});
		});
	}
}
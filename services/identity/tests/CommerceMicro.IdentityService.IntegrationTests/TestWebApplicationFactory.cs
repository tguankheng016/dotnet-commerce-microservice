using CommerceMicro.IdentityService.Api;
using CommerceMicro.IdentityService.Application.Startup;
using CommerceMicro.Modules.Postgres;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace CommerceMicro.IdentityService.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<IApplicationRoot>
{
	private readonly ITestOutputHelper _testOutputHelper;
	private readonly TestContainers _testContainers;

	public TestWebApplicationFactory(ITestOutputHelper testOutputHelper, TestContainers testContainers)
	{
		_testOutputHelper = testOutputHelper;
		_testContainers = testContainers;
	}

	protected override IHost CreateHost(IHostBuilder builder)
	{
		builder.UseSerilog(
			(ctx, loggerConfiguration) =>
			{
				if (_testOutputHelper is not null)
				{
					loggerConfiguration.WriteTo.TestOutput(
						_testOutputHelper,
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
					ConnectionString = _testContainers.DatabaseContainer.GetConnectionString()
				};

				services.AddSingleton(newPostgresOptions);
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
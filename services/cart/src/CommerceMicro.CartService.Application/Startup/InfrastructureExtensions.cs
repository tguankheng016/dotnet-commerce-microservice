using System.Reflection;
using System.Threading.RateLimiting;
using CommerceMicro.CartService.Application.Data;
using CommerceMicro.Modules.Caching;
using CommerceMicro.Modules.Core;
using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Core.Dependencies;
using CommerceMicro.Modules.Core.Sessions;
using CommerceMicro.Modules.Logging;
using CommerceMicro.Modules.MassTransit;
using CommerceMicro.Modules.OpenTelemetry;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Security;
using CommerceMicro.Modules.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using CommerceMicro.Modules.Mongo;
using CommerceMicro.Modules.Core.Exceptions;
using Serilog;
using CommerceMicro.Modules.Core.Persistences;
using CommerceMicro.CartService.Application.Data.Seed;
using CommerceMicro.Modules.Azure;

namespace CommerceMicro.CartService.Application.Startup;

public static class InfrastructureExtensions
{
	public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder, Assembly assembly)
	{
		var configuration = builder.Configuration;
		var env = builder.Environment;
		assembly = typeof(InfrastructureExtensions).Assembly;

		builder.LoadAzureKeyVault();

		var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
		Console.WriteLine(appOptions.Name);

		builder.Services.AddDefaultDependencyInjection(assembly);

		builder.Services.AddScoped<IAppSession, AppSession>();
		builder.Services.AddScoped<IDataSeeder, DataSeeder>();

		builder.Services.AddRateLimiter(options =>
		{
			options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
				RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
					factory: partition => new FixedWindowRateLimiterOptions
					{
						AutoReplenishment = true,
						PermitLimit = 100,
						QueueLimit = 0,
						Window = TimeSpan.FromSeconds(15)
					}));
		});

		// Setup Minimal API
		builder.Services.AddMinimalEndpoints(assembly);
		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddMongoDbContext<AppDbContext>(configuration);

		builder.AddCustomSerilog(env);

		builder.Services.AddCustomEasyCaching();

		builder.Services.AddCustomSwagger(configuration);
		builder.Services.AddCustomVersioning();

		builder.Services.AddCustomMediatR();

		builder.Services.AddValidatorsFromAssembly(assembly);

		builder.Services.AddProblemDetails();

		builder.Services.AddCustomMongoMassTransit(assembly);

		builder.Services.AddCustomJwtTokenHandler();
		builder.Services.AddCustomJwtAuthentication();

		builder.Services.AddPermissionAuthorization();

		builder.Services.AddHttpContextAccessor();

		builder.Services.AddGrpc(options =>
		{
			options.Interceptors.Add<GrpcExceptionInterceptor>();
		});

		builder.Services.AddGrpcClients();

		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		});

		builder.Services.AddCustomCors();

		builder.Services.AddCustomOpenTelemetry();

		return builder;
	}

	public static WebApplication UseInfrastructure(this WebApplication app)
	{
		var appOptions = app.GetOptions<AppOptions>(nameof(AppOptions));

		app.UseCustomCors();

		app.UseForwardedHeaders();

		app.UseCustomProblemDetails();

		app.UseSerilogRequestLogging(options =>
		{
			options.EnrichDiagnosticContext = LogEnrichHelper.EnrichFromRequest;
		});

		app.SeedData<AppDbContext>();

		app.UseRateLimiter();

		app.UseAuthentication();

		app.UseJwtTokenMiddleware();

		app.UsePermissionMiddleware();

		app.UseAuthorization();

		// Must come before custom swagger for versions to be visible in ui
		app.MapMinimalEndpoints();

		app.UseCustomSwagger();

		// Write app options name to http response
		app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

		return app;
	}
}

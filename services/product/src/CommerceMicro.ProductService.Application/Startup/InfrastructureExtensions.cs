using System.Reflection;
using System.Threading.RateLimiting;
using CommerceMicro.Modules.Logging;
using CommerceMicro.Modules.Caching;
using CommerceMicro.Modules.Core.Dependencies;
using CommerceMicro.Modules.Core.EFCore;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Core.Sessions;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Postgres;
using CommerceMicro.Modules.Security;
using CommerceMicro.Modules.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CommerceMicro.ProductService.Application.Data;
using CommerceMicro.Modules.MassTransit;

namespace CommerceMicro.ProductService.Application.Startup;

public static class InfrastructureExtensions
{
	public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder, Assembly assembly)
	{
		var configuration = builder.Configuration;
		var env = builder.Environment;
		assembly = typeof(InfrastructureExtensions).Assembly;

		builder.Services.AddDefaultDependencyInjection(assembly);

		builder.Services.AddScoped<IAppSession, AppSession>();

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

		builder.Services.AddNpgDbContext<AppDbContext>();

		builder.AddCustomSerilog(env);

		builder.Services.AddCustomEasyCaching();

		builder.Services.AddCustomSwagger(configuration);
		builder.Services.AddCustomVersioning();

		builder.Services.AddCustomMediatR();

		builder.Services.AddValidatorsFromAssembly(assembly);

		builder.Services.AddProblemDetails();

		builder.Services.AddCustomMassTransit<AppDbContext>(assembly);

		builder.Services.AddCustomJwtTokenHandler();
		builder.Services.AddCustomJwtAuthentication();

		builder.Services.AddPermissionAuthorization();

		builder.Services.AddHttpContextAccessor();

		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders =
				ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		});

		return builder;
	}

	public static WebApplication UseInfrastructure(this WebApplication app)
	{
		app.UseForwardedHeaders();

		app.UseCustomProblemDetails();

		app.UseSerilogRequestLogging(options =>
		{
			options.EnrichDiagnosticContext = LogEnrichHelper.EnrichFromRequest;
		});

		app.UseMigration<AppDbContext>();

		app.UseRateLimiter();

		app.UseAuthentication();

		app.UseJwtTokenMiddleware();

		app.UsePermissionMiddleware();

		app.UseAuthorization();

		// Must come before custom swagger for versions to be visible in ui
		app.MapMinimalEndpoints();

		app.UseCustomSwagger();

		return app;
	}
}

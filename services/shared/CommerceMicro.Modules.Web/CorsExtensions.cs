using CommerceMicro.Modules.Core;
using CommerceMicro.Modules.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.Modules.Web;

public static class CorsExtensions
{
	public static IServiceCollection AddCustomCors(this IServiceCollection services, string corsPolicy = "DefaultCORS")
	{
		var appOptions = services.GetOptions<AppOptions>(nameof(AppOptions));

		services.AddCors(
			options => options.AddPolicy(
				corsPolicy,
				policy => policy
					.WithOrigins(
						// App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
						appOptions.CorsOrigins
							.Split(",", StringSplitOptions.RemoveEmptyEntries)
							.Select(o => o)
							.ToArray()
					)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials()
			)
		);

		return services;
	}

	public static IApplicationBuilder UseCustomCors(this WebApplication app, string corsPolicy = "DefaultCORS")
	{
		app.UseCors(corsPolicy);

		return app;
	}
}

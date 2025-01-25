using CommerceMicro.Modules.Core.Configurations;
using CommerceMicro.Modules.Core.Persistences;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CommerceMicro.Modules.Mongo;

public static class MongoExtensions
{
	public static IServiceCollection AddMongoDbContext<TContext>(
		this IServiceCollection services, IConfiguration configuration, Action<MongoOptions>? configurator = null)
		where TContext : MongoDbContextBase, ICustomMongoDbContext
	{
		services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));

		if (configurator is { })
		{
			services.Configure(nameof(MongoOptions), configurator);
		}
		else
		{
			services.AddValidateOptions<MongoOptions>();
		}

		var mongoOptions = services.BuildServiceProvider().GetRequiredService<MongoOptions>();

		services.AddSingleton<IMongoClient>(new MongoClient(mongoOptions.ConnectionString));
		services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(mongoOptions.DatabaseName));
		services.AddScoped(typeof(TContext));
		services.AddScoped<ICustomMongoDbContext>(provider => provider.GetRequiredService<TContext>());

		return services;
	}

	public static IApplicationBuilder SeedData<TContext>(this IApplicationBuilder app)
		where TContext : MongoDbContextBase, ICustomMongoDbContext
	{
		using var scope = app.ApplicationServices.CreateScope();
		var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
		foreach (var seeder in seeders)
		{
			seeder.SeedAllAsync().GetAwaiter().GetResult();
		}

		return app;
	}
}

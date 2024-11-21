using System.Reflection;
using CommerceMicro.Modules.Core.EFCore;
using CommerceMicro.Modules.Core.Sessions;
using CommerceMicro.Modules.Postgres;
using CommerceMicro.ProductService.Application.Users.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommerceMicro.ProductService.Application.Data;

public class AppDbContext : NpgDbContextBase
{
	public AppDbContext(
		DbContextOptions<AppDbContext> options,
		ILogger<NpgDbContextBase>? logger = null,
		IAppSession? appSession = null) : base(options, logger, appSession)
	{
	}

	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(builder);

		builder.AddInboxStateEntity();
		builder.AddOutboxMessageEntity();
		builder.AddOutboxStateEntity();

		builder.SetSoftDeletedFilter();
		builder.ToSnakeCaseTableNames();
	}
}

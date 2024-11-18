using CommerceMicro.IdentityService.Application.Roles.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceMicro.IdentityService.Application.Data.Configurations;

public class RoleConfigurations : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.Property(r => r.Version).IsConcurrencyToken();
	}
}

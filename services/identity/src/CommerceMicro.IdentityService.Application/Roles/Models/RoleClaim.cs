using CommerceMicro.Modules.Core.Domain;
using Microsoft.AspNetCore.Identity;

namespace CommerceMicro.IdentityService.Application.Roles.Models;

public class RoleClaim : IdentityRoleClaim<long>, IAuditedEntity
{
	public virtual long Version { get; set; }

	public virtual long? CreatorUserId { get; set; }

	public virtual DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

	public virtual long? LastModifierUserId { get; set; }

	public virtual DateTimeOffset? LastModificationTime { get; set; }
}

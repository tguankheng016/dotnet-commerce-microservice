using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.IdentityService.Application.Users.Dtos;

public class UserDto : AuditedEntityDto<long>
{
    public required string UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public required string Email { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
}
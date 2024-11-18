using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.IdentityService.Application.Identities.Dtos;

public class UserLoginInfoDto : EntityDto<long>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }
}

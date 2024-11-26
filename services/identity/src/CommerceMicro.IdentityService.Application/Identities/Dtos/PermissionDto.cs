namespace CommerceMicro.IdentityService.Application.Identities.Dtos;

public class PermissionDto
{
    public string? Name { get; set; }

    public string? DisplayName { get; set; }

    public bool IsGranted { get; set; }
}

public class PermissionGroupDto
{
    public string? GroupName { get; set; }

    public IList<PermissionDto> Permissions { get; set; }

    public PermissionGroupDto()
    {
        Permissions = new List<PermissionDto>();
    }
}

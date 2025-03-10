using CommerceMicro.IdentityService.Application.Identities.Dtos;
using CommerceMicro.IdentityService.Application.Identities.Features.Authenticating.V2;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.Modules.Permissions;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.IdentityService.Application.Identities;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class IdentityMapper
{
	public partial UserLoginInfoDto UserToUserLoginInfoDto(User user);

	public partial AuthenticateCommand AuthenticateRequestToAuthenticateCommand(AuthenticateRequest request);

	public partial List<PermissionDto> PermissionsToPermissionDtos(List<Permission> permission);
}

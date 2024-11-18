using System.Diagnostics.CodeAnalysis;
using CommerceMicro.IdentityService.Application.Identities.Dtos;
using CommerceMicro.IdentityService.Application.Identities.Features.Authenticating.V2;
using CommerceMicro.IdentityService.Application.Users.Models;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.IdentityService.Application.Identities;

[Mapper]
public partial class IdentityMapper
{
	[SuppressMessage("Mapper", "RMG020")]
	public partial UserLoginInfoDto UserToUserLoginInfoDto(User user);

	public partial AuthenticateCommand AuthenticateRequestToAuthenticateCommand(AuthenticateRequest request);
}

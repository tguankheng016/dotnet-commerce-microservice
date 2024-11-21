using CommerceMicro.Modules.Security;
using Grpc.Core;

namespace CommerceMicro.IdentityService.Application.Identities.GrpcServer.Services;

public class IdentityGrpcServices : IdentityGrpcService.IdentityGrpcServiceBase
{
	private readonly ITokenKeyDbValidator _tokenKeyDbValidator;
	private readonly ITokenSecurityStampDbValidator _securityStampDbValidator;

	public IdentityGrpcServices(
		ITokenKeyDbValidator tokenKeyDbValidator,
		ITokenSecurityStampDbValidator securityStampDbValidator)
	{
		_tokenKeyDbValidator = tokenKeyDbValidator;
		_securityStampDbValidator = securityStampDbValidator;
	}

	public override async Task<GetValidateSecurityStampResponse> ValidateSecurityStamp(GetValidateSecurityStampRequest request, ServerCallContext context)
	{
		var isValid = await _securityStampDbValidator.ValidateSecurityStampFromDbAsync(request.CacheKey, request.UserId, request.SecurityStamp);

		return new GetValidateSecurityStampResponse()
		{
			IsValid = isValid
		};
	}

	public override async Task<GetValidateTokenKeyResponse> ValidateKey(GetValidateTokenKeyRequest request, ServerCallContext context)
	{
		var isValid = await _tokenKeyDbValidator.ValidateTokenKeyFromDbAsync(request.CacheKey, request.UserId, request.TokenKey);

		return new GetValidateTokenKeyResponse()
		{
			IsValid = isValid
		};
	}
}

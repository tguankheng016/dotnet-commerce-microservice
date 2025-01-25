using CommerceMicro.IdentityService;

namespace CommerceMicro.Modules.Security;

public class DefaultTokenSecurityStampDbValidator : ITokenSecurityStampDbValidator
{
	private readonly IdentityGrpcService.IdentityGrpcServiceClient _identityGrpcServiceClient;

	public DefaultTokenSecurityStampDbValidator(
		IdentityGrpcService.IdentityGrpcServiceClient identityGrpcServiceClient)
	{
		_identityGrpcServiceClient = identityGrpcServiceClient;
	}

	public async Task<bool> ValidateSecurityStampFromDbAsync(string cacheKey, string userId, string securityStamp)
	{
		var response = await _identityGrpcServiceClient.ValidateSecurityStampAsync(new GetValidateSecurityStampRequest()
		{
			CacheKey = cacheKey,
			UserId = userId,
			SecurityStamp = securityStamp
		});

		return response.IsValid;
	}
}

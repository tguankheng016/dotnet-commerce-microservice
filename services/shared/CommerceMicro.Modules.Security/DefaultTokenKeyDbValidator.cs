using CommerceMicro.IdentityService;

namespace CommerceMicro.Modules.Security;

public class DefaultTokenKeyDbValidator : ITokenKeyDbValidator
{
	private readonly IdentityGrpcService.IdentityGrpcServiceClient _identityGrpcServiceClient;

	public DefaultTokenKeyDbValidator(
		IdentityGrpcService.IdentityGrpcServiceClient identityGrpcServiceClient)
	{
		_identityGrpcServiceClient = identityGrpcServiceClient;
	}

	public async Task<bool> ValidateTokenKeyFromDbAsync(string cacheKey, long userId, string tokenKey)
	{
		var response = await _identityGrpcServiceClient.ValidateKeyAsync(new GetValidateTokenKeyRequest()
		{
			CacheKey = cacheKey,
			UserId = userId,
			TokenKey = tokenKey
		});

		return response.IsValid;
	}
}

namespace CommerceMicro.Modules.Security;

public class DefaultTokenSecurityStampDbValidator : ITokenSecurityStampDbValidator
{
    public Task<bool> ValidateSecurityStampFromDbAsync(string cacheKey, string userId, string securityStamp)
    {
        throw new NotImplementedException();
    }
}

namespace CommerceMicro.Modules.Security;

public class DefaultTokenKeyDbValidator : ITokenKeyDbValidator
{
    public Task<bool> ValidateTokenKeyFromDbAsync(string cacheKey, long userId, string tokenKey)
    {
        throw new NotImplementedException();
    }
}

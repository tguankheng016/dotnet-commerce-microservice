using CommerceMicro.IdentityService.Application.Data;
using CommerceMicro.Modules.Caching;
using CommerceMicro.Modules.Security;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.IdentityService.Application.Identities.Services;

public class TokenKeyDbValidator : ITokenKeyDbValidator
{
    private readonly AppDbContext _appDbContext;
    private readonly IEasyCachingProvider _cacheProvider;

    public TokenKeyDbValidator(
        AppDbContext appDbContext,
        ICacheManager cacheManager
    )
    {
        _appDbContext = appDbContext;
        _cacheProvider = cacheManager.GetCachingProvider();
    }

    public async Task<bool> ValidateTokenKeyFromDbAsync(string cacheKey, long userId, string tokenKey)
    {
        var isValid = await _appDbContext.UserTokens
            .AnyAsync(x =>
                x.UserId == userId && x.Name == tokenKey &&
                x.ExpireDate > DateTimeOffset.Now
            );

        if (isValid)
        {
            //cache only valid one because 1 user can have many token keys 
            await SetTokenKeyCacheAsync(cacheKey);
        }

        return isValid;
    }

    private async Task SetTokenKeyCacheAsync(string cacheKey)
    {
        await _cacheProvider.SetAsync(cacheKey, cacheKey, TimeSpan.FromHours(1));
    }
}
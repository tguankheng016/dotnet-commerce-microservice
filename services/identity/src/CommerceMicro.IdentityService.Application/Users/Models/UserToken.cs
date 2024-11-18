using Microsoft.AspNetCore.Identity;

namespace CommerceMicro.IdentityService.Application.Users.Models;

public class UserToken : IdentityUserToken<long>
{
    public virtual DateTimeOffset ExpireDate { get; set; }
}

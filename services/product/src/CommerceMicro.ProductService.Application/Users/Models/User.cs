using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.ProductService.Application.Users.Models;

public class User : Entity<long>
{
    public required string UserName { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}

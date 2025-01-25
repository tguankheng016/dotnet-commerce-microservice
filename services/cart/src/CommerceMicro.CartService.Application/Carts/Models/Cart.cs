using CommerceMicro.CartService.Application.Products.Models;
using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.CartService.Application.Carts.Models;

public class Cart : Entity<string>
{
	public required Product Product { get; set; }

	public int Quantity { get; set; }

	public long UserId { get; set; }

	public bool IsOutOfStock { get; set; }
}
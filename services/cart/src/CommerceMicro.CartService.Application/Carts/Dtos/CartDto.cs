using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.CartService.Application.Carts.Dtos;

public class CartDto : EntityDto<Guid>
{
	public long UserId { get; set; }

	public int ProductId { get; set; }

	public string? ProductName { get; set; }

	public string? ProductDescription { get; set; }

	public decimal ProductPrice { get; set; }

	public int Quantity { get; set; }

	public bool IsOutOfStock { get; set; }
}

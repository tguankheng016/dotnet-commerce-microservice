using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.CartService.Application.Products.Models;

public class Product : Entity
{
	public required string Name { get; set; }

	public string? Description { get; set; }

	public decimal Price { get; set; }

	public int StockQuantity { get; set; }

	public int? CategoryId { get; set; }

	public string? CategoryName { get; set; }
}

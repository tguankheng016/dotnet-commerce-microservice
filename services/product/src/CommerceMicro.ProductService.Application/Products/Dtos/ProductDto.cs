using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.ProductService.Application.Products.Dtos;

public class ProductDto : AuditedEntityDto
{
	public required string Name { get; set; }

	public required string Description { get; set; }

	public decimal Price { get; set; }

	public int StockQuantity { get; set; }

	public int? CategoryId { get; set; }

	public string? CategoryName { get; set; }
}

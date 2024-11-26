using System.ComponentModel.DataAnnotations;
using CommerceMicro.Modules.Core.Domain;
using CommerceMicro.ProductService.Application.Categories.Models;

namespace CommerceMicro.ProductService.Application.Products.Models;

public class Product : FullAuditedEntity
{
	public const int MaxProductNameLength = 50;
	public const int MaxProductDescriptionLength = 150;

	[StringLength(MaxProductNameLength)]
	public required string Name { get; set; }

	[StringLength(MaxProductDescriptionLength)]
	public required string Description { get; set; }

	public decimal Price { get; set; }

	public int StockQuantity { get; set; }

	public int? CategoryId { get; set; }

	public Category? CategoryFK { get; set; }
}

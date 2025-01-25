using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.ProductService.Application.Products.Dtos;

public class CreateOrEditProductDto : EntityDto<int?>
{
	public string? Name { get; set; }

	public string? Description { get; set; }

	public decimal? Price { get; set; }

	public int? StockQuantity { get; set; }

	public int? CategoryId { get; set; }
}

public class CreateProductDto : CreateOrEditProductDto;

public class EditProductDto : CreateOrEditProductDto;
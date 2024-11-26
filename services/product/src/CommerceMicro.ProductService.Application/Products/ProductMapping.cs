using CommerceMicro.ProductService.Application.Products.Dtos;
using CommerceMicro.ProductService.Application.Products.Features.CreatingProduct.V1;
using CommerceMicro.ProductService.Application.Products.Features.UpdatingProduct.V1;
using CommerceMicro.ProductService.Application.Products.Models;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.ProductService.Application.Products;

[Mapper]
public partial class ProductMapper
{
	public partial ProductDto ProductToProductDto(Product product);

	public partial CreateProductCommand CreateProductDtoToCreateProductCommand(CreateProductDto product);

	public partial Product CreateProductDtoToProduct(CreateProductDto product);

	public partial CreateOrEditProductDto ProductToCreateOrEditProductDto(Product product);

	public partial UpdateProductCommand EditProductDtoToUpdateProductCommand(EditProductDto product);

	public partial void EditProductDtoToProduct(EditProductDto editProductDto, Product product);
}

using CommerceMicro.ProductService.Application.Categories.Dtos;
using CommerceMicro.ProductService.Application.Categories.Features.CreatingCategory.V1;
using CommerceMicro.ProductService.Application.Categories.Features.UpdatingCategory.V1;
using CommerceMicro.ProductService.Application.Categories.Models;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.ProductService.Application.Categories;

[Mapper]
public partial class CategoryMapper
{
	public partial CategoryDto CategoryToCategoryDto(Category category);

	public partial CreateCategoryCommand CreateCategoryDtoToCreateCategoryCommand(CreateCategoryDto category);

	public partial Category CreateCategoryDtoToCategory(CreateCategoryDto category);

	public partial CreateOrEditCategoryDto CategoryToCreateOrEditCategoryDto(Category category);

	public partial UpdateCategoryCommand EditCategoryDtoToUpdateCategoryCommand(EditCategoryDto category);

	public partial void EditCategoryDtoToCategory(EditCategoryDto editCategoryDto, Category category);
}

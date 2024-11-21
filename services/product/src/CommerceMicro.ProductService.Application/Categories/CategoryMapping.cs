using CommerceMicro.ProductService.Application.Categories.Dtos;
using CommerceMicro.ProductService.Application.Categories.Models;
using Riok.Mapperly.Abstractions;

namespace CommerceMicro.ProductService.Application.Categories;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryDto CategoryToCategoryDto(Category category);
}

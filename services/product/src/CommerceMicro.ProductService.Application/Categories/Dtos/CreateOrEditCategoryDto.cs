using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.ProductService.Application.Categories.Dtos;

public class CreateOrEditCategoryDto : EntityDto<int?>
{
    public string? CategoryName { get; set; }
}

public class CreateCategoryDto : CreateOrEditCategoryDto;

public class EditCategoryDto : CreateOrEditCategoryDto;
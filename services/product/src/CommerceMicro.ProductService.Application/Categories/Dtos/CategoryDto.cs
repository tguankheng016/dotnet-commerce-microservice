using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.ProductService.Application.Categories.Dtos;

public class CategoryDto : AuditedEntityDto
{
    public required string CategoryName { get; set; }
}
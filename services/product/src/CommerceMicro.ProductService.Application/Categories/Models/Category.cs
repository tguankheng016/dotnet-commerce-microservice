using System.ComponentModel.DataAnnotations;
using CommerceMicro.Modules.Core.Domain;

namespace CommerceMicro.ProductService.Application.Categories.Models;

public class Category : FullAuditedEntity
{
    public const int MaxCategoryNameLength = 50;

    [Required]
    [StringLength(MaxCategoryNameLength)]
    public virtual required string CategoryName { get; set; }
}

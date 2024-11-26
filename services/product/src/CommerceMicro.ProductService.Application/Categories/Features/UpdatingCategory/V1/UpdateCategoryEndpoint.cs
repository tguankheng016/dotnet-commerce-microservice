using CommerceMicro.ProductService.Application.Categories.Dtos;
using CommerceMicro.ProductService.Application.Data;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.EFCore;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Web;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.ProductService.Application.Categories.Features.UpdatingCategory.V1;

public class UpdateCategoryEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPut($"{EndpointConfig.BaseApiPath}/products/category", Handle)
			.RequireAuthorization(CategoryPermissions.Pages_Categories_Edit)
			.WithName("UpdateCategory")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<UpdateCategoryResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Update New Category")
			.WithDescription("Update New Category")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromBody] EditCategoryDto request
	)
	{
		var mapper = new CategoryMapper();
		var command = mapper.EditCategoryDtoToUpdateCategoryCommand(request);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Result
public record UpdateCategoryResult(CategoryDto Category);

// Command
public class UpdateCategoryCommand : EditCategoryDto, ICommand<UpdateCategoryResult>, ITransactional;

// Validator
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
	public UpdateCategoryValidator()
	{
		RuleFor(x => x.Id).NotEmpty().WithMessage("Invalid category id");
		RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid category id");
		RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Please enter the category name");
	}
}

// Handler
internal class UpdateCategoryHandler(
	AppDbContext appDbContext
) : ICommandHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
	public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
	{
		var category = await appDbContext.Categories
			.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

		if (category is null)
		{
			throw new NotFoundException("Category not found");
		}

		var mapper = new CategoryMapper();

		mapper.EditCategoryDtoToCategory(command, category);

		var categoryResult = appDbContext.Categories.Update(category);

		var categoryDto = mapper.CategoryToCategoryDto(category);

		return new UpdateCategoryResult(categoryDto);
	}
}
using CommerceMicro.ProductService.Application.Categories.Dtos;
using CommerceMicro.ProductService.Application.Data;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.EFCore;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Web;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.ProductService.Application.Categories.Features.CreatingCategory.V1;

public class CreateCategoryEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPost($"{EndpointConfig.BaseApiPath}/products/category", Handle)
			.RequireAuthorization(CategoryPermissions.Pages_Categories_Create)
			.WithName("CreateCategory")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<CreateCategoryResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Create New Category")
			.WithDescription("Create New Category")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[FromBody] CreateCategoryDto request
		)
	{
		var mapper = new CategoryMapper();
		var command = mapper.CreateCategoryDtoToCreateCategoryCommand(request);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Result
public record CreateCategoryResult(CategoryDto Category);

// Command
public class CreateCategoryCommand : CreateCategoryDto, ICommand<CreateCategoryResult>, ITransactional;

// Validator
public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
	public CreateCategoryValidator()
	{
		RuleFor(x => x.Id).Must(x => x is null || x == 0).WithMessage("Invalid category id");
		RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Please enter the category name");
	}
}

// Handler
internal class CreateCategoryHandler(
	AppDbContext appDbContext
) : ICommandHandler<CreateCategoryCommand, CreateCategoryResult>
{
	public async Task<CreateCategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
	{
		var mapper = new CategoryMapper();
		var category = mapper.CreateCategoryDtoToCategory(command);

		await appDbContext.AddAsync(category, cancellationToken: cancellationToken);
		await appDbContext.SaveChangesAsync(cancellationToken);

		var categoryDto = mapper.CategoryToCategoryDto(category);

		return new CreateCategoryResult(categoryDto);
	}
}
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
using CommerceMicro.ProductService.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.ProductService.Application.Categories.Features.DeletingCategory.V1;

public class DeleteCategoryEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapDelete($"{EndpointConfig.BaseApiPath}/products/category/{{categoryid:int}}", Handle)
			.RequireAuthorization(CategoryPermissions.Pages_Categories_Delete)
			.WithName("DeleteCategory")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Delete Category")
			.WithDescription("Delete Category")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] int CategoryId
	)
	{
		var command = new DeleteCategoryCommand(CategoryId);

		await mediator.Send(command, cancellationToken);

		return Results.Ok();
	}
}

// Result
public record DeleteCategoryResult();

// Command
public record DeleteCategoryCommand(int Id) : ICommand<DeleteCategoryResult>, ITransactional;

// Validator
public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
	public DeleteCategoryValidator()
	{
		RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid category id");
	}
}

// Handler
internal class DeleteCategoryHandler(
	AppDbContext appDbContext
) : ICommandHandler<DeleteCategoryCommand, DeleteCategoryResult>
{
	public async Task<DeleteCategoryResult> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
	{
		var category = await appDbContext.Categories
			.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

		if (category is null)
		{
			throw new NotFoundException("Category not found");
		}

		appDbContext.Categories.Remove(category);

		return new DeleteCategoryResult();
	}
}
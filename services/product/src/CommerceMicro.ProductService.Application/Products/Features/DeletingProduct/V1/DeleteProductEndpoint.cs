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
using MassTransit;
using CommerceMicro.Modules.Contracts;

namespace CommerceMicro.ProductService.Application.Products.Features.DeletingProduct.V1;

public class DeleteProductEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapDelete($"{EndpointConfig.BaseApiPath}/products/product/{{productid:int}}", Handle)
			.RequireAuthorization(ProductPermissions.Pages_Products_Delete)
			.WithName("DeleteProduct")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Delete Product")
			.WithDescription("Delete Product")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] int ProductId
	)
	{
		var command = new DeleteProductCommand(ProductId);

		await mediator.Send(command, cancellationToken);

		return Results.Ok();
	}
}

// Result
public record DeleteProductResult();

// Command
public record DeleteProductCommand(int Id) : ICommand<DeleteProductResult>, ITransactional;

// Validator
public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
	public DeleteProductValidator()
	{
		RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid product id");
	}
}

// Handler
internal class DeleteProductHandler(
	AppDbContext appDbContext,
	IPublishEndpoint publishEndpoint
) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
	public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
	{
		var product = await appDbContext.Products
			.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

		if (product is null)
		{
			throw new NotFoundException("Product not found");
		}

		appDbContext.Products.Remove(product);

		await publishEndpoint.Publish(
			new ProductDeletedEvent(
				product.Id
			),
			cancellationToken
		);

		return new DeleteProductResult();
	}
}
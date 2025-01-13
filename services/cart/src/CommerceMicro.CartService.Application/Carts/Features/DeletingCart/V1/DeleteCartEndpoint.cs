using CommerceMicro.CartService.Application.Carts.Models;
using CommerceMicro.CartService.Application.Data;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Core.Sessions;
using CommerceMicro.Modules.Mongo;
using CommerceMicro.Modules.Web;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

namespace CommerceMicro.CartService.Application.Carts.Features.DeletingCart.V1;

public class DeleteCartEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapDelete($"{EndpointConfig.BaseApiPath}/carts/cart/{{cartid:Guid}}", Handle)
			.RequireAuthorization()
			.WithName("DeleteCart")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Delete Cart")
			.WithDescription("Delete Cart")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] Guid CartId
	)
	{
		var command = new DeleteCartCommand(CartId);

		await mediator.Send(command, cancellationToken);

		return Results.Ok();
	}
}

// Result
public record DeleteCartResult();

// Command
public record DeleteCartCommand(Guid Id) : ICommand<DeleteCartResult>;

// Validator
public class DeleteCartValidator : AbstractValidator<DeleteCartCommand>
{
	public DeleteCartValidator()
	{
		RuleFor(x => x.Id).NotEmpty().WithMessage("Invalid cart id");
	}
}

// Handler
internal class DeleteCartHandler(
	AppDbContext appDbContext,
	IAppSession appSession,
	IPublishEndpoint publishEndpoint
) : ICommandHandler<DeleteCartCommand, DeleteCartResult>
{
	public async Task<DeleteCartResult> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
	{
		var userId = appSession.UserId;

		if (!userId.HasValue)
		{
			throw new UnAuthorizedException("User not found");
		}

		var cart = await appDbContext.Carts.FirstOrDefaultAsync(x => x.Id == command.Id.ToString() && x.UserId == userId.Value, cancellationToken);

		if (cart is null)
		{
			throw new NotFoundException("Cart not found");
		}

		var filter = Builders<Cart>.Filter.Where(x => x.Id == command.Id.ToString() && x.UserId == userId.Value);

		await appDbContext.Carts.DeleteOneAsync(filter, cancellationToken);

		await publishEndpoint.Publish(
			new ChangeProductQuantityEvent(
				cart.Product.Id,
				cart.Quantity
			),
			cancellationToken
		);

		return new DeleteCartResult();
	}
}
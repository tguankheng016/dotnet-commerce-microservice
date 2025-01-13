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

namespace CommerceMicro.CartService.Application.Carts.Features.AddingCart.V1;

public class AddCartEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPost($"{EndpointConfig.BaseApiPath}/carts/cart", Handle)
			.RequireAuthorization()
			.WithName("AddCart")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<AddCartResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Add Cart")
			.WithDescription("Add Cart")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[FromBody] AddCartDto request
		)
	{
		var command = new AddCartCommand(request.ProductId);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Dto
public record AddCartDto(int ProductId);

// Result
public record AddCartResult();

// Command
public record AddCartCommand(int ProductId) : ICommand<AddCartResult>;

// Validator
public class AddCartCommandValidator : AbstractValidator<AddCartCommand>
{
	public AddCartCommandValidator()
	{
		RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Invalid product id");
	}
}

// Handler
internal class AddCartHandler(
	AppDbContext appDbContext,
	IAppSession appSession,
	IPublishEndpoint publishEndpoint
) : ICommandHandler<AddCartCommand, AddCartResult>
{
	public async Task<AddCartResult> Handle(AddCartCommand command, CancellationToken cancellationToken)
	{
		var userId = appSession.UserId;

		if (!userId.HasValue)
		{
			throw new UnAuthorizedException("User not found");
		}

		var product = await appDbContext.Products.FirstOrDefaultAsync(x => x.Id == command.ProductId, cancellationToken);

		if (product is null)
		{
			throw new NotFoundException("Invalid product id");
		}

		var cart = await appDbContext.Carts.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.Product.Id == command.ProductId, cancellationToken);

		if (cart is null)
		{
			var newCartItem = new Cart()
			{
				Id = Guid.NewGuid().ToString(),
				Product = product,
				Quantity = 1,
				UserId = userId.Value
			};

			await appDbContext.Carts.InsertOneAsync(newCartItem, cancellationToken: cancellationToken);
		}
		else
		{
			await appDbContext.Carts.UpdateOneAsync(
					x => x.UserId == userId.Value && x.Product.Id == command.ProductId,
					Builders<Cart>.Update
						.Set(x => x.Quantity, cart.Quantity + 1)
				);
		}

		await publishEndpoint.Publish(
			new ChangeProductQuantityEvent(
				product.Id,
				-1
			),
			cancellationToken
		);

		return new AddCartResult();
	}
}

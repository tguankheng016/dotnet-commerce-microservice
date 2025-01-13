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

namespace CommerceMicro.CartService.Application.Carts.Features.UpdatingCart.V1;

public class UpdateCartEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPut($"{EndpointConfig.BaseApiPath}/carts/cart", Handle)
			.RequireAuthorization()
			.WithName("UpdateCart")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<UpdateCartResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Update Cart")
			.WithDescription("Update Cart")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[FromBody] UpdateCartDto request
		)
	{
		var command = new UpdateCartCommand(request.Id, request.Quantity);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Dto
public record UpdateCartDto(Guid Id, int Quantity);

// Result
public record UpdateCartResult();

// Command
public record UpdateCartCommand(Guid Id, int Quantity) : ICommand<UpdateCartResult>;

// Validator
public class UpdateCartCommandValidator : AbstractValidator<UpdateCartCommand>
{
	public UpdateCartCommandValidator()
	{
		RuleFor(x => x.Id).NotEmpty().WithMessage("Invalid cart id");
		RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Invalid quantity");
	}
}

// Handler
internal class UpdateCartHandler(
	AppDbContext appDbContext,
	IAppSession appSession,
	IPublishEndpoint publishEndpoint
) : ICommandHandler<UpdateCartCommand, UpdateCartResult>
{
	public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
	{
		var cartId = command.Id.ToString();
		var userId = appSession.UserId;

		if (!userId.HasValue)
		{
			throw new UnAuthorizedException("User not found");
		}

		var cart = await appDbContext.Carts.FirstOrDefaultAsync(x => x.Id == cartId && x.UserId == userId.Value, cancellationToken);

		if (cart is null)
		{
			throw new NotFoundException("Invalid cart id");
		}

		var product = await appDbContext.Products.FirstOrDefaultAsync(x => x.Id == cart.Product.Id, cancellationToken);

		if (product is null)
		{
			throw new NotFoundException("Invalid product id");
		}

		int quantityChanged = 0;

		if (command.Quantity == 0)
		{
			quantityChanged = cart.Quantity;

			var filter = Builders<Cart>.Filter.Where(x => x.Id == cartId);

			await appDbContext.Carts.DeleteOneAsync(filter, cancellationToken);
		}
		else if (command.Quantity != cart.Quantity)
		{
			quantityChanged = cart.Quantity - command.Quantity;

			if (quantityChanged < 0)
			{
				if (product.StockQuantity == 0)
				{
					throw new BadRequestException("This product is out of stock");
				}

				if (-1 * quantityChanged > product.StockQuantity)
				{
					throw new BadRequestException("The selected quantity exceeds the remaining stock");
				}
			}

			await appDbContext.Carts.UpdateOneAsync(
				x => x.UserId == userId.Value && x.Id == cartId,
				Builders<Cart>.Update
					.Set(x => x.Quantity, command.Quantity)
			);
		}

		if (quantityChanged != 0)
		{
			await publishEndpoint.Publish(
				new ChangeProductQuantityEvent(
					product.Id,
					quantityChanged
				),
				cancellationToken
			);
		}

		return new UpdateCartResult();
	}
}
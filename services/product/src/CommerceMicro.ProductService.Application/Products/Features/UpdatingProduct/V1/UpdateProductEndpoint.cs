using CommerceMicro.ProductService.Application.Products.Dtos;
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
using CommerceMicro.ProductService.Application.Products.Models;
using MassTransit;
using CommerceMicro.Modules.Contracts;

namespace CommerceMicro.ProductService.Application.Products.Features.UpdatingProduct.V1;

public class UpdateProductEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPut($"{EndpointConfig.BaseApiPath}/products/product", Handle)
			.RequireAuthorization(ProductPermissions.Pages_Products_Edit)
			.WithName("UpdateProduct")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<UpdateProductResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Update New Product")
			.WithDescription("Update New Product")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromBody] EditProductDto request
	)
	{
		var mapper = new ProductMapper();
		var command = mapper.EditProductDtoToUpdateProductCommand(request);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Result
public record UpdateProductResult(ProductDto Product);

// Command
public class UpdateProductCommand : EditProductDto, ICommand<UpdateProductResult>, ITransactional;

// Validator
public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
	public UpdateProductValidator()
	{
		RuleFor(x => x.Id).NotEmpty().WithMessage("Invalid product id");
		RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid product id");

		RuleFor(x => x.Name).NotEmpty().WithMessage("Please enter the product name");
		RuleFor(x => x.Name).MaximumLength(Product.MaxProductNameLength)
			.WithMessage($"Product name length cannot be greater than {Product.MaxProductNameLength}");

		RuleFor(x => x.Description).NotEmpty().WithMessage("Please enter the product name");
		RuleFor(x => x.Description).MaximumLength(Product.MaxProductDescriptionLength)
			.WithMessage($"Product description length cannot be greater than {Product.MaxProductDescriptionLength}");

		RuleFor(x => x.Price).NotEmpty().WithMessage("Please enter the product price");
		RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Product price must be greater than 0");

		RuleFor(x => x.StockQuantity).NotEmpty().WithMessage("Please enter the product stock quantity");
		RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Product stock quantity must be greater than 0");
	}
}

// Handler
internal class UpdateProductHandler(
	AppDbContext appDbContext,
	IPublishEndpoint publishEndpoint
) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
	public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
	{
		var product = await appDbContext.Products
			.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

		if (product is null)
		{
			throw new NotFoundException("Product not found");
		}

		var mapper = new ProductMapper();

		mapper.EditProductDtoToProduct(command, product);

		var productResult = appDbContext.Products.Update(product);

		var productDto = mapper.ProductToProductDto(product);

		await publishEndpoint.Publish(
			new ProductUpdatedEvent(
				product.Id,
				product.Name,
				product.Description,
				product.Price,
				product.StockQuantity
			),
			cancellationToken
		);

		return new UpdateProductResult(productDto);
	}
}
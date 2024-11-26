using CommerceMicro.ProductService.Application.Products.Dtos;
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
using CommerceMicro.ProductService.Application.Products.Models;

namespace CommerceMicro.ProductService.Application.Products.Features.CreatingProduct.V1;

public class CreateProductEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPost($"{EndpointConfig.BaseApiPath}/products/product", Handle)
			.RequireAuthorization(ProductPermissions.Pages_Products_Create)
			.WithName("CreateProduct")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<CreateProductResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Create New Product")
			.WithDescription("Create New Product")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[FromBody] CreateProductDto request
		)
	{
		var mapper = new ProductMapper();
		var command = mapper.CreateProductDtoToCreateProductCommand(request);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Result
public record CreateProductResult(ProductDto Product);

// Command
public class CreateProductCommand : CreateProductDto, ICommand<CreateProductResult>, ITransactional;

// Validator
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
	public CreateProductValidator()
	{
		RuleFor(x => x.Id).Must(x => x is null || x == 0).WithMessage("Invalid product id");

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
internal class CreateProductHandler(
	AppDbContext appDbContext
) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
	public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
	{
		var mapper = new ProductMapper();
		var product = mapper.CreateProductDtoToProduct(command);

		await appDbContext.AddAsync(product, cancellationToken: cancellationToken);
		await appDbContext.SaveChangesAsync(cancellationToken);

		var productDto = mapper.ProductToProductDto(product);

		// TODO: publish event to cart

		return new CreateProductResult(productDto);
	}
}
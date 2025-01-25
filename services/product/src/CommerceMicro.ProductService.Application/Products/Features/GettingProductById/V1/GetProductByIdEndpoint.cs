using CommerceMicro.ProductService.Application.Products.Dtos;
using CommerceMicro.Modules.Core.CQRS;
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
using CommerceMicro.ProductService.Application.Data;

namespace CommerceMicro.ProductService.Application.Products.Features.GettingProductById.V1;

public class GetProductByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/products/product/{{productid:int}}", Handle)
            .RequireAuthorization(ProductPermissions.Pages_Products)
            .WithName("GetProductById")
            .WithApiVersionSet(builder.GetApiVersionSet())
            .Produces<GetProductByIdResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id")
            .WithOpenApi()
            .HasLatestApiVersion();

        return builder;
    }

    async Task<IResult> Handle(
        IMediator mediator, CancellationToken cancellationToken,
        [FromRoute] int productId
    )
    {
        var query = new GetProductByIdQuery(productId);

        var result = await mediator.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}

// Result
public record GetProductByIdResult(CreateOrEditProductDto Product);

// Query
public record GetProductByIdQuery(int Id) : IQuery<GetProductByIdResult>;

// Validator
public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThanOrEqualTo(0).WithMessage("Invalid product id");
    }
}

// Handler
internal class GetProductByIdHandler(
    AppDbContext appDbContext
) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        CreateOrEditProductDto productEditDto;

        if (request.Id == 0)
        {
            // Create
            productEditDto = new CreateOrEditProductDto();
        }
        else
        {
            // Edit
            var product = await appDbContext.Products
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (product is null)
            {
                throw new NotFoundException("Product not found");
            }

            var mapper = new ProductMapper();

            productEditDto = mapper.ProductToCreateOrEditProductDto(product);
        }

        return new GetProductByIdResult(productEditDto);
    }
}
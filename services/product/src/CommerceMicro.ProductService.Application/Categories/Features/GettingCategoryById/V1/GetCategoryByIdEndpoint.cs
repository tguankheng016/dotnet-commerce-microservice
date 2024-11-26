using CommerceMicro.ProductService.Application.Categories.Dtos;
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

namespace CommerceMicro.ProductService.Application.Categories.Features.GettingCategoryById.V1;

public class GetCategoryByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/products/category/{{categoryid:int}}", Handle)
            .RequireAuthorization(CategoryPermissions.Pages_Categories)
            .WithName("GetCategoryById")
            .WithApiVersionSet(builder.GetApiVersionSet())
            .Produces<GetCategoryByIdResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Category By Id")
            .WithDescription("Get Category By Id")
            .WithOpenApi()
            .HasLatestApiVersion();

        return builder;
    }

    async Task<IResult> Handle(
        IMediator mediator, CancellationToken cancellationToken,
        [FromRoute] int categoryId
    )
    {
        var query = new GetCategoryByIdQuery(categoryId);

        var result = await mediator.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}

// Result
public record GetCategoryByIdResult(CreateOrEditCategoryDto Category);

// Query
public record GetCategoryByIdQuery(int Id) : IQuery<GetCategoryByIdResult>;

// Validator
public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThanOrEqualTo(0).WithMessage("Invalid category id");
    }
}

// Handler
internal class GetCategoryByIdHandler(
    AppDbContext appDbContext
) : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
{
    public async Task<GetCategoryByIdResult> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        CreateOrEditCategoryDto categoryEditDto;

        if (request.Id == 0)
        {
            // Create
            categoryEditDto = new CreateOrEditCategoryDto();
        }
        else
        {
            // Edit
            var category = await appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (category is null)
            {
                throw new NotFoundException("Category not found");
            }

            var mapper = new CategoryMapper();

            categoryEditDto = mapper.CategoryToCreateOrEditCategoryDto(category);
        }

        return new GetCategoryByIdResult(categoryEditDto);
    }
}
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.Pagination;
using CommerceMicro.Modules.Web;
using CommerceMicro.ProductService.Application.Products.Dtos;
using CommerceMicro.ProductService.Application.Data;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using CommerceMicro.Modules.Core.Queryable;
using CommerceMicro.ProductService.Application.Products.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MediatR;

namespace CommerceMicro.ProductService.Application.Products.Features.GettingProducts.V1;

public class GetProductsEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapGet($"{EndpointConfig.BaseApiPath}/products/products", Handle)
			.WithName("GetProducts")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<GetProductsResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get All Products")
			.WithDescription("Get All Products")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[AsParameters] GetProductsRequest request
		)
	{
		var query = new GetProductsQuery()
		{
			SkipCount = request.SkipCount ?? 0,
			MaxResultCount = request.MaxResultCount ?? 0,
			Filters = request.Filters,
			Sorting = request.Sorting,
			CategoryIdFilter = request.CategoryIdFilter
		};

		var result = await mediator.Send(query, cancellationToken);

		return Results.Ok(result);
	}
}

// Request
public class GetProductsRequest() : PageRequest
{
	public int? CategoryIdFilter { get; set; }
}

// Result
public class GetProductsResult : PagedResultDto<ProductDto>;

// Query
public class GetProductsQuery : PageQuery<GetProductsResult>
{
	public int? CategoryIdFilter { get; set; }
}

// Validator
public class GetProductsValidator : AbstractValidator<GetProductsQuery>
{
	public GetProductsValidator()
	{
		RuleFor(x => x.SkipCount)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Page should at least greater than or equal to 0.");

		RuleFor(x => x.MaxResultCount)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Page size should at least greater than or equal to 0.");
	}
}

// Handler
internal class GetProductsHandler(
	AppDbContext appDbContext
) : IQueryHandler<GetProductsQuery, GetProductsResult>
{
	public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
	{
		var results = new List<ProductDto>();

		var filteredProducts = appDbContext.Products.AsNoTracking()
			.Include(x => x.CategoryFK)
			.WhereIf(!string.IsNullOrWhiteSpace(request.Filters),
				e =>
					e.Name!.ToUpper().Contains(request.Filters!.ToUpper()) ||
					e.Description!.ToUpper().Contains(request.Filters!.ToUpper())
			)
			.WhereIf(request.CategoryIdFilter.HasValue, e => e.CategoryId == request.CategoryIdFilter);

		IQueryable<Product>? pagedAndFilteredProducts = null;

		if (request.SkipCount == 0 && request.MaxResultCount == 0)
		{
			pagedAndFilteredProducts = filteredProducts
				.OrderBy(string.IsNullOrEmpty(request.Sorting) ? "id asc" : request.Sorting);
		}
		else
		{
			pagedAndFilteredProducts = filteredProducts
				.OrderBy(string.IsNullOrEmpty(request.Sorting) ? "id asc" : request.Sorting)
				.PageBy(request);
		}

		var totalCount = await filteredProducts.CountAsync(cancellationToken: cancellationToken);

		var dbList = await pagedAndFilteredProducts.ToListAsync(cancellationToken: cancellationToken);

		var mapper = new ProductMapper();

		foreach (var o in dbList)
		{
			var res = mapper.ProductToProductDto(o);
			res.CategoryName = o.CategoryFK?.CategoryName;
			results.Add(res);
		}

		var pagedResults = new GetProductsResult()
		{
			TotalCount = totalCount,
			Items = results
		};

		return pagedResults;
	}
}
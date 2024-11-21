using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.Pagination;
using CommerceMicro.Modules.Web;
using CommerceMicro.ProductService.Application.Categories.Dtos;
using CommerceMicro.ProductService.Application.Data;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using CommerceMicro.Modules.Core.Queryable;
using CommerceMicro.ProductService.Application.Categories.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MediatR;

namespace CommerceMicro.ProductService.Application.Categories.Features.GettingCategories.V1;

public class GetCategoriesEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapGet($"{EndpointConfig.BaseApiPath}/products/categories", Handle)
			.RequireAuthorization()
			.WithName("GetCategories")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<GetCategoriesResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get All Categories")
			.WithDescription("Get All Categories")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[AsParameters] GetCategoriesRequest request
		)
	{
		var query = new GetCategoriesQuery()
		{
			SkipCount = request.SkipCount ?? 0,
			MaxResultCount = request.MaxResultCount ?? 0,
			Filters = request.Filters,
			Sorting = request.Sorting
		};

		var result = await mediator.Send(query, cancellationToken);

		return Results.Ok(result);
	}
}

// Request
public class GetCategoriesRequest() : PageRequest;

// Result
public class GetCategoriesResult : PagedResultDto<CategoryDto>;

// Query
public class GetCategoriesQuery : PageQuery<GetCategoriesResult>;

// Validator
public class GetCategoriesValidator : AbstractValidator<GetCategoriesQuery>
{
	public GetCategoriesValidator()
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
internal class GetCategoriesHandler(
	AppDbContext appDbContext
) : IQueryHandler<GetCategoriesQuery, GetCategoriesResult>
{
	public async Task<GetCategoriesResult> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
	{
		var results = new List<CategoryDto>();

		var filteredCategories = appDbContext.Categories.AsNoTracking()
			.WhereIf(!string.IsNullOrWhiteSpace(request.Filters),
				e =>
					e.CategoryName!.Contains(request.Filters!)
			);

		IQueryable<Category>? pagedAndFilteredCategories = null;

		if (request.SkipCount == 0 && request.MaxResultCount == 0)
		{
			pagedAndFilteredCategories = filteredCategories
				.OrderBy(string.IsNullOrEmpty(request.Sorting) ? "id asc" : request.Sorting);
		}
		else
		{
			pagedAndFilteredCategories = filteredCategories
				.OrderBy(string.IsNullOrEmpty(request.Sorting) ? "id asc" : request.Sorting)
				.PageBy(request);
		}

		var totalCount = await filteredCategories.CountAsync(cancellationToken: cancellationToken);

		var dbList = await pagedAndFilteredCategories.ToListAsync(cancellationToken: cancellationToken);

		var mapper = new CategoryMapper();

		foreach (var o in dbList)
		{
			var res = mapper.CategoryToCategoryDto(o);
			results.Add(res);
		}

		var pagedResults = new GetCategoriesResult()
		{
			TotalCount = totalCount,
			Items = results
		};

		return pagedResults;
	}
}
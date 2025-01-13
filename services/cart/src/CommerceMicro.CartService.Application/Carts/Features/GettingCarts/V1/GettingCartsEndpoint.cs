using CommerceMicro.CartService.Application.Carts.Dtos;
using CommerceMicro.CartService.Application.Carts.Models;
using CommerceMicro.CartService.Application.Data;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Core.Pagination;
using CommerceMicro.Modules.Core.Sessions;
using CommerceMicro.Modules.Web;
using FluentValidation;
using CommerceMicro.Modules.Mongo;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MediatR;

namespace CommerceMicro.CartService.Application.Carts.Features.GettingCarts.V1;

public class GettingCartsEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapGet($"{EndpointConfig.BaseApiPath}/carts/carts", Handle)
			.RequireAuthorization()
			.WithName("GetCarts")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<GetCartsResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get All Carts")
			.WithDescription("Get All Carts")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
			IMediator mediator, CancellationToken cancellationToken,
			[AsParameters] GetCartsRequest request
		)
	{
		var query = new GetCartsQuery()
		{
			SkipCount = request.SkipCount ?? 0,
			MaxResultCount = request.MaxResultCount ?? 0,
			Filters = request.Filters,
			Sorting = request.Sorting,
		};

		var result = await mediator.Send(query, cancellationToken);

		return Results.Ok(result);
	}
}

// Request
public class GetCartsRequest() : PageRequest
{
}

// Result
public class GetCartsResult : PagedResultDto<CartDto>;

// Query
public class GetCartsQuery : PageQuery<GetCartsResult>
{
	public int? CategoryIdFilter { get; set; }
}

// Validator
public class GetCartsValidator : AbstractValidator<GetCartsQuery>
{
	public GetCartsValidator()
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
internal class GetCartsHandler(
	AppDbContext appDbContext,
	IAppSession appSession
) : IQueryHandler<GetCartsQuery, GetCartsResult>
{
	public async Task<GetCartsResult> Handle(GetCartsQuery request, CancellationToken cancellationToken)
	{
		var userId = appSession.UserId;

		if (!userId.HasValue)
		{
			throw new UnAuthorizedException("User not found");
		}

		var results = new List<CartDto>();

		var filter = Builders<Cart>.Filter.Where(x => x.UserId == userId.Value);

		var filteredCarts = appDbContext.Carts.Find(filter);

		var pagedAndFilteredCarts = filteredCarts
			.SortBy(string.IsNullOrEmpty(request.Sorting) ? "creationTime desc" : request.Sorting);

		if (request.SkipCount > 0 || request.MaxResultCount > 0)
		{
			pagedAndFilteredCarts = pagedAndFilteredCarts.PageBy(request);
		}

		var totalCount = (int)await filteredCarts.CountDocumentsAsync(cancellationToken);

		var dbList = await pagedAndFilteredCarts.ToListAsync(cancellationToken);

		foreach (var o in dbList)
		{
			var res = new CartDto()
			{
				Id = new Guid(o.Id),
				UserId = o.UserId,
				ProductId = o.Product.Id,
				ProductName = o.Product.Name,
				ProductDescription = o.Product.Description,
				ProductPrice = o.Product.Price,
				Quantity = o.Quantity,
				IsOutOfStock = o.IsOutOfStock
			};
			results.Add(res);
		}

		var pagedResults = new GetCartsResult
		{
			TotalCount = totalCount,
			Items = results
		};

		return pagedResults;
	}
}
using CommerceMicro.IdentityService.Application.Identities.Dtos;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.IdentityService.Application.Identities.Features.GettingAllPermissions.V1;

public class GetAllPermissionsEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapGet($"{EndpointConfig.BaseApiPath}/identities/permissions", Handle)
			.RequireAuthorization()
			.WithName("GetAllPermissions")
			.WithApiVersionSet(builder.NewApiVersionSet(IdentityConfigs.ApiVersionSet).Build())
			.Produces<GetAllPermissionsResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get All Permissions")
			.WithDescription("Get All Permissions")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken
	)
	{
		var query = new GetAllPermissionsQuery();

		var result = await mediator.Send(query, cancellationToken);

		return Results.Ok(result);
	}
}

// Results
public record GetAllPermissionsResult(IList<PermissionGroupDto> Items);

// Query
public record GetAllPermissionsQuery() : IQuery<GetAllPermissionsResult>;

// Handlers
internal class GetAllPermissionsHandler(
	AppPermissions permissionList
) : IQueryHandler<GetAllPermissionsQuery, GetAllPermissionsResult>
{
	public Task<GetAllPermissionsResult> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
	{
		var allPermissions = permissionList.Items.ToList();

		var permissionGroups = allPermissions.GroupBy(x => x.Group);

		var results = new List<PermissionGroupDto>();

		var mapper = new IdentityMapper();

		foreach (var g in permissionGroups)
		{
			var res = new PermissionGroupDto()
			{
				GroupName = g.Key,
				Permissions = mapper.PermissionsToPermissionDtos(g.ToList())
			};

			results.Add(res);
		}

		return Task.FromResult(new GetAllPermissionsResult(results));
	}
}

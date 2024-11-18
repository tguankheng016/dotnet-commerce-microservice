using CommerceMicro.IdentityService.Application.Users.Dtos;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Web;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CommerceMicro.IdentityService.Application.Users.Features.GettingUserById.V1;

public class GetUserByIdEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapGet($"{EndpointConfig.BaseApiPath}/identities/user/{{userid:long}}", Handle)
			.RequireAuthorization(UserPermissions.Pages_Administration_Users)
			.WithName("GetUserById")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<GetUserByIdResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Get User By Id")
			.WithDescription("Get User By Id")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] long UserId
	)
	{
		var query = new GetUserByIdQuery(UserId);

		var result = await mediator.Send(query, cancellationToken);

		return Results.Ok(result);
	}
}

// Result
public record GetUserByIdResult(CreateOrEditUserDto User);

// Query
public record GetUserByIdQuery(long Id) : IQuery<GetUserByIdResult>;

// Validator
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
	public GetUserByIdQueryValidator()
	{
		RuleFor(x => x.Id).GreaterThanOrEqualTo(0).WithMessage("Invalid user id");
	}
}

// Handler
internal class GetUserByIdHandler(
	UserManager<User> userManager
) : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
{
	public async Task<GetUserByIdResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		CreateOrEditUserDto userEditDto;

		if (request.Id == 0)
		{
			// Create
			userEditDto = new CreateOrEditUserDto();
		}
		else
		{
			// Edit
			var user = await userManager.Users
				.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

			if (user is null)
			{
				throw new NotFoundException("User not found");
			}

			var mapper = new UserMapper();

			userEditDto = mapper.UserToCreateOrEditUserDto(user);
			userEditDto.Roles = await userManager.GetRolesAsync(user);
		}

		return new GetUserByIdResult(userEditDto);
	}
}
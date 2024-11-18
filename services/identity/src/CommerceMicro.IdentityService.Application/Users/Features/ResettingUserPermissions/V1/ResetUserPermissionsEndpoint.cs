using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.IdentityService.Application.Users.Services;
using CommerceMicro.IdentityService.Application.Data;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.EFCore;
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

namespace CommerceMicro.IdentityService.Application.Users.Features.ResettingUserPermissions.V1;

public class ResetUserPermissionsEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPut($"{EndpointConfig.BaseApiPath}/identities/user/{{userid:long}}/reset-permissions", Handle)
			.RequireAuthorization(UserPermissions.Pages_Administration_Users_ChangePermissions)
			.WithName("ResetUserPermissions")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<ResetUserPermissionsResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Reset User Permissions")
			.WithDescription("Reset User Permissions")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] long UserId
	)
	{
		var query = new ResetUserPermissionsCommand(UserId);

		await mediator.Send(query, cancellationToken);

		return Results.Ok(new ResetUserPermissionsResult());
	}
}

// Result
public record ResetUserPermissionsResult();

// Command
public record ResetUserPermissionsCommand(long UserId) : ICommand<ResetUserPermissionsResult>, ITransactional;

// Validator
public class ResetUserPermissionsValidator : AbstractValidator<ResetUserPermissionsCommand>
{
	public ResetUserPermissionsValidator()
	{
		RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Invalid user id");
	}
}

// Handler
internal class ResetUserPermissionsHandler(
	UserManager<User> userManager,
	AppDbContext appDbContext,
	IUserRolePermissionManager userRolePermissionManager
) : ICommandHandler<ResetUserPermissionsCommand, ResetUserPermissionsResult>
{
	public async Task<ResetUserPermissionsResult> Handle(ResetUserPermissionsCommand command, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByIdAsync(command.UserId.ToString());

		if (user == null)
		{
			throw new NotFoundException("User not found");
		}

		var userSpecificPermissions = await appDbContext.UserRolePermissions
			.Where(x => x.UserId == user.Id)
			.ToListAsync(cancellationToken);

		appDbContext.UserRolePermissions.RemoveRange(userSpecificPermissions);

		// Reset User Permission Cache
		await userRolePermissionManager.SetUserPermissionAsync(user.Id, cancellationToken);

		return new ResetUserPermissionsResult();
	}
}
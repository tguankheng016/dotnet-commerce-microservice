using CommerceMicro.IdentityService.Application.Roles.Models;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.IdentityService.Application.Users.Services;
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

namespace CommerceMicro.IdentityService.Application.Roles.Features.DeletingRole.V1;

public class DeleteRoleEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapDelete($"{EndpointConfig.BaseApiPath}/identities/role/{{roleid:long}}", Handle)
			.RequireAuthorization(RolePermissions.Pages_Administration_Roles_Delete)
			.WithName("DeleteRole")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Delete Role")
			.WithDescription("Delete Role")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] long RoleId
	)
	{
		var command = new DeleteRoleCommand(RoleId);

		await mediator.Send(command, cancellationToken);

		return Results.Ok();
	}
}

// Result
public record DeleteRoleResult();

// Command
public record DeleteRoleCommand(long Id) : ICommand<DeleteRoleResult>, ITransactional;

// Validator
public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
	public DeleteRoleValidator()
	{
		RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid role id");
	}
}

// Handler
internal class DeleteRoleHandler(
	RoleManager<Role> roleManager,
	UserManager<User> userManager,
	IUserRolePermissionManager userRolePermissionManager
) : ICommandHandler<DeleteRoleCommand, DeleteRoleResult>
{
	public async Task<DeleteRoleResult> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
	{
		var role = await roleManager.FindByIdAsync(command.Id.ToString());

		if (role is null)
		{
			throw new NotFoundException("Role not found");
		}

		if (role.IsStatic)
		{
			throw new BadRequestException("You cannot delete static role!");
		}

		var users = await userManager.GetUsersInRoleAsync(role.Name!);

		foreach (var user in users)
		{
			var removeUserRoleResult = await userManager.RemoveFromRoleAsync(user, role.Name!);

			if (!removeUserRoleResult.Succeeded)
			{
				throw new BadRequestException(string.Join(',', removeUserRoleResult.Errors.Select(e => e.Description)));
			}

			await userRolePermissionManager.RemoveUserRoleCacheAsync(user.Id, cancellationToken);
		}

		var roleDeleteResult = await roleManager.DeleteAsync(role);

		if (!roleDeleteResult.Succeeded)
		{
			throw new BadRequestException(string.Join(',', roleDeleteResult.Errors.Select(e => e.Description)));
		}

		return new DeleteRoleResult();
	}
}
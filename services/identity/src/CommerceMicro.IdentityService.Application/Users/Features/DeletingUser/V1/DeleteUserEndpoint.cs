using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.Modules.Caching;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Core.EFCore;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Core.Sessions;
using CommerceMicro.Modules.Permissions;
using CommerceMicro.Modules.Security.Caching;
using CommerceMicro.Modules.Web;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.IdentityService.Application.Users.Features.DeletingUser.V1;

public class DeleteUserEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapDelete($"{EndpointConfig.BaseApiPath}/identities/user/{{userid:long}}", Handle)
			.RequireAuthorization(UserPermissions.Pages_Administration_Users_Delete)
			.WithName("DeleteUser")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Delete User")
			.WithDescription("Delete User")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromRoute] long UserId
	)
	{
		var command = new DeleteUserCommand(UserId);

		await mediator.Send(command, cancellationToken);

		return Results.Ok();
	}
}

// Result
public record DeleteUserResult();

// Command
public record DeleteUserCommand(long Id) : ICommand<DeleteUserResult>, ITransactional;

// Validator
public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
	public DeleteUserValidator()
	{
		RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid user id");
	}
}

// Handler
internal class DeleteUserHandler(
	UserManager<User> userManager,
	IAppSession appSession,
	ICacheManager cacheManager,
	IPublishEndpoint publishEndpoint
) : ICommandHandler<DeleteUserCommand, DeleteUserResult>
{
	public async Task<DeleteUserResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByIdAsync(command.Id.ToString());

		if (user is null)
		{
			throw new NotFoundException("User not found");
		}

		if (user.UserName == UserConsts.DefaultUsername.Admin)
		{
			throw new BadRequestException("You cannot delete admin account!");
		}

		if (user.Id == appSession.UserId)
		{
			throw new BadRequestException("You cannot delete your own account!");
		}

		// TODO: Temporary Solution because of unique index username and soft delete issue
		var deletedUsername = user.UserName!;
		user.UserName += "_DELETED";

		await userManager.UpdateAsync(user);

		await userManager.DeleteAsync(user);

		var _cacheProvider = cacheManager.GetCachingProvider();

		// Invalidate Deleted User Tokens
		await _cacheProvider.RemoveAsync(SecurityStampCacheItem.GenerateCacheKey(user.Id.ToString()), cancellationToken);

		await publishEndpoint.Publish(
			new UserDeletedEvent(
				user.Id,
				deletedUsername
			),
			cancellationToken
		);

		return new DeleteUserResult();
	}
}

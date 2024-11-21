using System.Security.Claims;
using CommerceMicro.IdentityService.Application.Identities.Http;
using CommerceMicro.IdentityService.Application.Identities.Services;
using CommerceMicro.IdentityService.Application.Roles.Constants;
using CommerceMicro.IdentityService.Application.Roles.Models;
using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Core.CQRS;
using CommerceMicro.Modules.Security;
using CommerceMicro.Modules.Web;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PasswordGenerator;

namespace CommerceMicro.IdentityService.Application.Identities.Features.OAuthAuthenticating.V1;

public class OAuthAuthenticateEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPost($"{EndpointConfig.BaseApiPath}/identities/oauth-authenticate", Handle)
			.WithName("OAuthAuthenticate")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<OAuthAuthenticateResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("OauthAuthenticate")
			.WithDescription("OAuthAuthenticate")
			.WithOpenApi()
			.HasLatestApiVersion();

		return builder;
	}

	async Task<IResult> Handle(
		IMediator mediator, CancellationToken cancellationToken,
		[FromBody] OAuthAuthenticateRequest request
	)
	{
		var command = new OAuthAuthenticateCommand(request.Code, request.RedirectUri);

		var result = await mediator.Send(command, cancellationToken);

		return Results.Ok(result);
	}
}

// Requests
public record OAuthAuthenticateRequest(
	string? Code,
	string? RedirectUri
);

// Results
public record OAuthAuthenticateResult(string AccessToken, int ExpireInSeconds, string RefreshToken, int RefreshTokenExpireInSeconds);

// Command
public record OAuthAuthenticateCommand(
	string? Code,
	string? RedirectUri
) : ICommand<OAuthAuthenticateResult>;

// Validators
public class OAuthAuthenticateValidator : AbstractValidator<OAuthAuthenticateCommand>
{
	public OAuthAuthenticateValidator()
	{
		RuleFor(x => x.Code).NotEmpty().WithMessage("Code cannot be empty!");
		RuleFor(x => x.RedirectUri).NotEmpty().WithMessage("Redirect uri cannot be empty!");
	}
}

// Handlers
internal class OAuthAuthenticateHandler(
	UserManager<User> userManager,
	RoleManager<Role> roleManager,
	IOAuthApiClient oAuthApiClient,
	IJwtTokenGenerator jwtTokenGenerator,
	IUserClaimsPrincipalFactory<User> userClaimsPrincipal,
	IPublishEndpoint publishEndpoint,
	ILogger<OAuthAuthenticateHandler> logger
) : ICommandHandler<OAuthAuthenticateCommand, OAuthAuthenticateResult>
{
	public async Task<OAuthAuthenticateResult> Handle(OAuthAuthenticateCommand request, CancellationToken cancellationToken)
	{
		var tokenResponse = await oAuthApiClient.ConnectTokenAsync(request.Code!, request.RedirectUri!);

		var userInfo = await oAuthApiClient.ConnectUserInfoAsync(tokenResponse.Access_Token!);
		var externalUserId = new Guid(userInfo.Sub!);

		var user = await userManager.Users
			.FirstOrDefaultAsync(x => x.ExternalUserId == externalUserId, cancellationToken);

		if (user == null && userInfo.Preferred_Username != UserConsts.DefaultUsername.Admin)
		{
			// Create New User
			var newUser = new User
			{
				UserName = userInfo.Preferred_Username,
				FirstName = userInfo.Given_Name ?? "",
				LastName = userInfo.Family_Name ?? "",
				Email = userInfo.Email,
				EmailConfirmed = userInfo.Email_Verified,
			};

			var result = await userManager.CreateAsync(newUser, GenerateRandomPassword());

			if (result.Succeeded)
			{
				var defaultRole = await roleManager.Roles
					.FirstOrDefaultAsync(x => x.IsDefault, cancellationToken);

				await userManager.AddToRoleAsync(newUser, defaultRole?.Name ?? RoleConsts.RoleName.User);

				await publishEndpoint.Publish(
					new UserCreatedEvent(
						user!.Id,
						user.UserName!,
						user.FirstName ?? "",
						user.LastName ?? ""
					),
					cancellationToken
				);
			}
		}
		else
		{
			// Auto Link For Admin Case Only
			if (user == null && userInfo.Preferred_Username == UserConsts.DefaultUsername.Admin)
			{
				user = await userManager.Users
					.FirstAsync(x => x.UserName == UserConsts.DefaultUsername.Admin, cancellationToken);

				user.ExternalUserId = externalUserId;
			}

			// Existing User
			// Try Update
			try
			{
				user!.UserName = userInfo.Preferred_Username;
				user.Email = userInfo.Email;
				user.EmailConfirmed = userInfo.Email_Verified;
				user.FirstName = userInfo.Given_Name ?? "";
				user.LastName = userInfo.Family_Name ?? "";

				await userManager.UpdateAsync(user);

				await publishEndpoint.Publish(
					new UserUpdatedEvent(
						user!.Id,
						user.UserName!,
						user.FirstName ?? "",
						user.LastName ?? ""
					),
					cancellationToken
				);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"An error occured when login openiddict to update user info - {userInfo.Preferred_Username}");
			}
		}

		var principal = await userClaimsPrincipal.CreateAsync(user!);
		var claimIdentity = principal.Identity as ClaimsIdentity;

		var refreshToken = await jwtTokenGenerator
			.CreateRefreshToken(claimIdentity!, user!);

		var accessToken = await jwtTokenGenerator
			.CreateAccessToken(claimIdentity!, user!, refreshTokenKey: refreshToken.Key);

		return new OAuthAuthenticateResult(
			accessToken,
			(int)TokenConsts.AccessTokenExpiration.TotalSeconds,
			refreshToken.Token,
			(int)TokenConsts.RefreshTokenExpiration.TotalSeconds
		);
	}

	private string GenerateRandomPassword()
	{
		var pwd = new Password(16);
		return pwd.Next();
	}
}
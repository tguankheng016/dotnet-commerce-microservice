using System.Security.Claims;
using Asp.Versioning.Conventions;
using CommerceMicro.IdentityService.Application.Identities.Services;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.Modules.Core.Exceptions;
using CommerceMicro.Modules.Core.Validations;
using CommerceMicro.Modules.Security;
using CommerceMicro.Modules.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CommerceMicro.IdentityService.Application.Identities.Features.Authenticating.V1;

public class AuthenticateEndpoint : IMinimalEndpoint
{
	public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
	{
		builder.MapPost($"{EndpointConfig.BaseApiPath}/identities/authenticate", Handle)
			.WithName("Authenticate")
			.WithApiVersionSet(builder.GetApiVersionSet())
			.Produces<AuthenticateResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.WithSummary("Authenticate")
			.WithDescription("Authenticate")
			.WithOpenApi()
			.HasApiVersion(1.0);

		return builder;
	}

	async Task<IResult> Handle(
		UserManager<User> userManager,
		SignInManager<User> signInManager,
		IJwtTokenGenerator jwtTokenGenerator,
		IUserClaimsPrincipalFactory<User> userClaimsPrincipal,
		CancellationToken cancellationToken,
		[FromBody] AuthenticateRequest request
	)
	{
		var validator = new AuthenticateValidator();
		await validator.HandleValidationAsync(request);

		var identityUser = await userManager.FindByNameAsync(request.UsernameOrEmailAddress!)
			?? await userManager.FindByEmailAsync(request.UsernameOrEmailAddress!);

		if (identityUser is null)
		{
			throw new BadRequestException($"Invalid username or password!");
		}

		var signInResult = await signInManager.CheckPasswordSignInAsync(identityUser, request.Password!, false);

		if (signInResult.IsLockedOut)
		{
			throw new BadRequestException($"Your account has been temporarily locked due to multiple unsuccessful login attempts.");
		}

		if (!signInResult.Succeeded)
		{
			throw new BadRequestException($"Invalid username or password!");
		}

		var principal = await userClaimsPrincipal.CreateAsync(identityUser);
		var claimIdentity = principal.Identity as ClaimsIdentity;

		var refreshToken = await jwtTokenGenerator
			.CreateRefreshToken(claimIdentity!, identityUser);

		var accessToken = await jwtTokenGenerator
			.CreateAccessToken(claimIdentity!, identityUser, refreshTokenKey: refreshToken.Key);

		var result = new AuthenticateResult(
			accessToken,
			(int)TokenConsts.AccessTokenExpiration.TotalSeconds,
			refreshToken.Token,
			(int)TokenConsts.RefreshTokenExpiration.TotalSeconds
		);
		return Results.Ok(result);
	}
}

public class AuthenticateValidator : AbstractValidator<AuthenticateRequest>
{
	public AuthenticateValidator()
	{
		RuleFor(x => x.UsernameOrEmailAddress).NotEmpty().WithMessage("Please enter the username or email address");
		RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter the password");
	}
}

public record AuthenticateRequest(
	string? UsernameOrEmailAddress,
	string? Password
);

public record AuthenticateResult(string AccessToken, int ExpireInSeconds, string RefreshToken, int RefreshTokenExpireInSeconds);

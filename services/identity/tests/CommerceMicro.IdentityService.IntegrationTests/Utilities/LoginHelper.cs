using System.Security.Claims;
using CommerceMicro.IdentityService.Application.Identities.Services;
using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.Application.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceMicro.IdentityService.IntegrationTests.Utilities;

public static class LoginHelper
{
	public static async Task<HttpClient> LoginAs(this TestWebApplicationFactory apiFactory, string username)
	{
		var scope = apiFactory.Services.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
		var userClaimsPrincipal = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<User>>();
		var tokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

		var user = await userManager.FindByNameAsync(username);
		var principal = await userClaimsPrincipal.CreateAsync(user!);
		var claimIdentity = principal.Identity as ClaimsIdentity;
		var token = await tokenGenerator.CreateAccessToken(claimIdentity!, user!);

		var client = apiFactory.CreateClient();
		client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

		return client;
	}

	public static Task<HttpClient> LoginAsAdmin(this TestWebApplicationFactory apiFactory)
	{
		return LoginAs(apiFactory, UserConsts.DefaultUsername.Admin);
	}

	public static Task<HttpClient> LoginAsUser(this TestWebApplicationFactory apiFactory)
	{
		return LoginAs(apiFactory, UserConsts.DefaultUsername.User);
	}
}
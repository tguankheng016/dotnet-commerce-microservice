using CommerceMicro.IdentityService.Application.Identities.Features.GettingCurrentSession.V1;
using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.IntegrationTests.Utilities;
using CommerceMicro.Modules.Permissions;
using Xunit.Abstractions;

namespace CommerceMicro.IdentityService.IntegrationTests.Identities;

[Collection(IdentityTestCollection1.Name)]
public class GetCurrentSession_Tests : AppTestBase
{
	protected override string EndpointName { get; } = "current-session";

	public GetCurrentSession_Tests(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}

	[Theory]
	[InlineData(UserConsts.DefaultUsername.Admin)]
	[InlineData(UserConsts.DefaultUsername.User)]
	[InlineData(null)]
	public async Task Should_Get_User_Session_Test(string username)
	{
		// Arrange
		HttpClient? client;

		if (username is not null)
		{
			client = await ApiFactory.LoginAs(username);
		}
		else
		{
			client = Client;
		}

		// Act
		var response = await client.GetAsync(Endpoint);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var currentSession = await response.Content.ReadFromJsonAsync<GetCurrentSessionResult>();
		currentSession.Should().NotBeNull();

		if (username is null)
		{
			currentSession!.User.Should().BeNull();
		}
		else
		{
			currentSession!.User.Should().NotBeNull();
			currentSession!.User!.UserName.Should().Be(username);

			var allPermissions = AppPermissionProvider.GetPermissions();

			currentSession.AllPermissions.Count().Should().Be(allPermissions.Count());

			if (currentSession.User.UserName == UserConsts.DefaultUsername.Admin)
			{
				currentSession.GrantedPermissions.Count().Should().Be(allPermissions.Count());
			}
		}
	}
}

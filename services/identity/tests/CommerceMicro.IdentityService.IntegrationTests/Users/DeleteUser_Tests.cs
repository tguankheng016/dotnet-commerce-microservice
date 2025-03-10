using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.IdentityService.IntegrationTests.Utilities;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Permissions;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CommerceMicro.IdentityService.IntegrationTests.Users;

[Collection(UserTestCollection1.Name)]
public class DeleteUserTestBase : AppTestBase
{
	protected override string EndpointName { get; } = "user";

	protected DeleteUserTestBase(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}
}


public class DeleteUser_Tests : DeleteUserTestBase
{
	public DeleteUser_Tests(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}

	[Fact]
	public async Task Should_Delete_User_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var newUser = GetTestUser();

		await DbContext.Users.AddAsync(newUser);
		await DbContext.SaveChangesAsync();

		var totalCount = await DbContext.Users.CountAsync();

		var newUserClient = await ApiFactory.LoginAs(newUser.UserName!);
		await newUserClient.DeleteAsync($"{Endpoint}/{newUser.Id}");

		await TestHarness.Start();

		// Act
		var response = await client.DeleteAsync($"{Endpoint}/{newUser.Id}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var anyUserDeletedFound = await DbContext.Users.AnyAsync(x => x.Id == newUser.Id);
		anyUserDeletedFound.Should().BeFalse();

		var newTotalCount = await DbContext.Users.CountAsync();
		newTotalCount.Should().Be(totalCount - 1);

		var newUserResponse = await newUserClient.DeleteAsync($"{Endpoint}/{newUser.Id}");
		newUserResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

		var publishedMessage = await TestHarness.Published.SelectAsync<UserDeletedEvent>().FirstOrDefault();
		publishedMessage.Should().NotBeNull();

		var userDeletedEvent = publishedMessage.Context.Message;
		userDeletedEvent.Id.Should().Be(newUser.Id);
		userDeletedEvent.UserName.Should().Be(newUser.UserName);

		await TestHarness.Stop();
	}

	[Fact]
	public async Task Should_Delete_Own_User_Account_With_Error_Test()
	{
		// Arrange
		var newUser = GetTestUser();

		await DbContext.Users.AddAsync(newUser);
		await DbContext.SaveChangesAsync();

		await DbContext.UserRolePermissions.AddAsync(new UserRolePermission()
		{
			Id = 0,
			UserId = newUser.Id,
			Name = UserPermissions.Pages_Administration_Users_Delete,
			IsGranted = true
		});
		await DbContext.SaveChangesAsync();

		await TestHarness.Start();

		var client = await ApiFactory.LoginAs(newUser.UserName!);

		// Act
		var response = await client.DeleteAsync($"{Endpoint}/{newUser.Id}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse?.Detail.Should().Be("You cannot delete your own account!");

		var publishedMessage = await TestHarness.Published.SelectAsync<UserDeletedEvent>().FirstOrDefault();
		publishedMessage.Should().BeNull();

		await TestHarness.Stop();
	}

	[Fact]
	public async Task Should_Delete_Admin_User_With_Error_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();

		await TestHarness.Start();

		// Act
		var response = await client.DeleteAsync($"{Endpoint}/1");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse?.Detail.Should().Be("You cannot delete admin account!");

		var publishedMessage = await TestHarness.Published.SelectAsync<UserDeletedEvent>().FirstOrDefault();
		publishedMessage.Should().BeNull();

		await TestHarness.Stop();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(100)]
	public async Task Should_Delete_User_With_Invalid_UserId_Test(long userId)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();

		// Act
		var response = await client.DeleteAsync($"{Endpoint}/{userId}");

		// Assert
		response.StatusCode.Should().Be(userId > 0 ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
	}

	[Fact]
	public async Task Should_Delete_User_With_Unauthorized_Error_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsUser();
		var newUser = GetTestUser();

		await DbContext.Users.AddAsync(newUser);
		await DbContext.SaveChangesAsync();

		// Act
		var response = await client.DeleteAsync($"{Endpoint}/{newUser.Id}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
	}

	private User GetTestUser()
	{
		return UserFaker.GetUserFaker().Generate();
	}
}

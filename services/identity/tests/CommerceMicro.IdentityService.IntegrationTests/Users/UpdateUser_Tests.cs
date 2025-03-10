using System.Collections;
using CommerceMicro.IdentityService.Application.Roles.Constants;
using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.Application.Users.Dtos;
using CommerceMicro.IdentityService.Application.Users.Features.UpdatingUser.V1;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.IdentityService.IntegrationTests.Utilities;
using CommerceMicro.Modules.Contracts;
using CommerceMicro.Modules.Permissions.Caching;
using MassTransit.Internals;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using static Bogus.DataSets.Name;

namespace CommerceMicro.IdentityService.IntegrationTests.Users;

[Collection(UserTestCollection1.Name)]
public class UpdateUserTestBase : AppTestBase
{
	protected override string EndpointName { get; } = "user";

	protected UpdateUserTestBase(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}
}

public class UpdateUser_Tests : UpdateUserTestBase
{
	public UpdateUser_Tests(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}

	[Fact]
	public async Task Should_Update_User_Test()
	{
		// Arrange
		var newUser = UserFaker.GetUserFaker().Generate();
		await DbContext.Users.AddAsync(newUser);
		await DbContext.SaveChangesAsync();
		var userId = newUser.Id;

		var client = await ApiFactory.LoginAsAdmin();
		var totalCount = await DbContext.Users.CountAsync();
		var testUser = new Faker<EditUserDto>()
			.RuleFor(x => x.Id, userId)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, f => f.Internet.UserName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);
		var request = testUser.Generate();
		request.Id = newUser.Id;
		request.Roles = new List<string>() { RoleConsts.RoleName.User };

		await TestHarness.Start();

		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var updateResult = await response.Content.ReadFromJsonAsync<UpdateUserResult>();
		updateResult.Should().NotBeNull();
		updateResult!.User.Should().NotBeNull();
		updateResult!.User.Id.Should().Be(userId);
		updateResult!.User.UserName.Should().Be(request.UserName);
		updateResult!.User.FirstName.Should().Be(request.FirstName);
		updateResult!.User.LastName.Should().Be(request.LastName);
		updateResult!.User.Email.Should().Be(request.Email);

		var newTotalCount = await DbContext.Users.CountAsync();
		newTotalCount.Should().Be(totalCount);

		var publishedMessage = await TestHarness.Published.SelectAsync<UserUpdatedEvent>().FirstOrDefault();
		publishedMessage.Should().NotBeNull();

		var userUpdatedEvent = publishedMessage.Context.Message;
		userUpdatedEvent.Id.Should().Be(updateResult.User.Id);
		userUpdatedEvent.UserName.Should().Be(updateResult.User.UserName);
		userUpdatedEvent.FirstName.Should().Be(updateResult.User.FirstName);
		userUpdatedEvent.LastName.Should().Be(updateResult.User.LastName);

		await TestHarness.Stop();
	}

	[Fact]
	public async Task Should_Update_User_With_Roles_Test()
	{
		// Arrange
		var newUser = UserFaker.GetUserFaker().Generate();
		await DbContext.Users.AddAsync(newUser);
		await DbContext.SaveChangesAsync();

		var userId = newUser.Id;
		var userRole = await DbContext.Roles.FirstAsync(x => x.Name == RoleConsts.RoleName.User);

		var newUserRole = new UserRole
		{
			UserId = newUser.Id,
			RoleId = userRole.Id
		};

		await DbContext.UserRoles.AddAsync(newUserRole);
		await DbContext.SaveChangesAsync();

		var client = await ApiFactory.LoginAsAdmin();
		var totalCount = await DbContext.Users.CountAsync();
		var testUser = new Faker<EditUserDto>()
			.RuleFor(x => x.Id, userId)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, f => f.Internet.UserName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);
		var request = testUser.Generate();
		var updatedRoles = new List<string>();
		request.Roles = updatedRoles;

		var user = await DbContext.Users.FirstAsync(x => x.Id == userId);

		await TestHarness.Start();

		// Prepare the edited user caches
		HttpClient? userClient = await ApiFactory.LoginAs(user.UserName!);
		await userClient.PutAsJsonAsync(Endpoint, request);
		var userRoleCaches = await CacheProvider.GetAsync<UserRoleCacheItem>(UserRoleCacheItem.GenerateCacheKey(userId));
		userRoleCaches.Should().NotBeNull();
		userRoleCaches.HasValue.Should().BeTrue();

		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var updateResult = await response.Content.ReadFromJsonAsync<UpdateUserResult>();
		updateResult.Should().NotBeNull();
		updateResult!.User.Should().NotBeNull();
		updateResult!.User.Id.Should().Be(newUser.Id);
		updateResult!.User.UserName.Should().Be(request.UserName);
		updateResult!.User.FirstName.Should().Be(request.FirstName);
		updateResult!.User.LastName.Should().Be(request.LastName);
		updateResult!.User.Email.Should().Be(request.Email);
		updateResult!.User.Roles.Should().BeEquivalentTo(updatedRoles);

		// Validate Caches
		userRoleCaches = await CacheProvider.GetAsync<UserRoleCacheItem>(UserRoleCacheItem.GenerateCacheKey(userId));
		userRoleCaches.Should().NotBeNull();
		userRoleCaches.HasValue.Should().BeFalse();

		var newTotalCount = await DbContext.Users.CountAsync();
		newTotalCount.Should().Be(totalCount);

		var publishedMessage = (await TestHarness.Published.SelectAsync<UserUpdatedEvent>().ToListAsync())
			.Where(x => x.Context.Message.Id == updateResult.User.Id)
			.FirstOrDefault();
		publishedMessage.Should().NotBeNull();

		var userUpdatedEvent = publishedMessage!.Context.Message;
		userUpdatedEvent.Id.Should().Be(updateResult.User.Id);
		userUpdatedEvent.UserName.Should().Be(updateResult.User.UserName);
		userUpdatedEvent.FirstName.Should().Be(updateResult.User.FirstName);
		userUpdatedEvent.LastName.Should().Be(updateResult.User.LastName);

		await TestHarness.Stop();
	}
}

public class UpdateUserValidation_Tests : UpdateUserTestBase
{
	public UpdateUserValidation_Tests(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}

	[Fact]
	public async Task Should_Update_User_With_Unauthorized_Error_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsUser();
		var testUser = new Faker<EditUserDto>()
			.RuleFor(x => x.Id, 2)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, f => f.Internet.UserName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);
		var request = testUser.Generate();

		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
	}

	[Theory]
	[InlineData(2, null, "admin@testgk.com", "Email 'admin@testgk.com' is already taken.")]
	[InlineData(2, UserConsts.DefaultUsername.Admin, null, "Username 'admin' is already taken.")]
	public async Task Should_Not_Update_User_With_Duplicate_Username_Or_Email_Test(long userId, string? username, string? email, string errorMessage)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var testUser = new Faker<EditUserDto>()
			.RuleFor(x => x.Id, userId)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, username)
			.RuleFor(x => x.Email, email)
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);

		if (username is null)
		{
			testUser.RuleFor(x => x.UserName, (f) => f.Internet.UserName());
		}

		if (email is null)
		{
			testUser.RuleFor(x => x.Email, f => f.Internet.Email());
		}

		var request = testUser.Generate();

		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse!.Detail.Should().Be(errorMessage);
	}

	[Theory]
	[ClassData(typeof(GetValidateUserUpdateTestData))]
	public async Task Should_Update_Role_With_Invalid_Input_Test(EditUserDto request, string errorMessage)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse!.Detail.Should().Be(errorMessage);
	}

	private static EditUserDto GetUpdateUserRequest(int scenario)
	{
		var testUser = new Faker<EditUserDto>()
			.RuleFor(x => x.Id, 2)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, f => f.Internet.UserName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);

		switch (scenario)
		{
			case 0:
				{
					// Email Not Filled In
					testUser.RuleFor(u => u.Email, "");
					break;
				}
			case 2:
				{
					// Password not same as confirm password
					testUser.RuleFor(u => u.ConfirmPassword, (f, u) => u.Password + "Wrong");
					break;
				}
			case 3:
				{
					// Invalid Email
					testUser.RuleFor(u => u.Email, f => f.Internet.UserName());
					break;
				}
			case 4:
				{
					// Exceed length
					testUser.RuleFor(u => u.FirstName, f => f.Internet.Password(length: 1000));
					break;
				}
			case 5:
				{
					// Invalid Id
					testUser.RuleFor(u => u.Id, 0);
					break;
				}
			case 6:
				{
					// Invalid Id
					testUser.RuleFor(u => u.Id, (int?)null);
					break;
				}
		}

		return testUser.Generate();
	}

	private class GetValidateUserUpdateTestData : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[]
			{
				GetUpdateUserRequest(0),
				"Please enter the email address"
			};
			yield return new object[]
			{
				GetUpdateUserRequest(2),
				"Passwords should match"
			};
			yield return new object[]
			{
				GetUpdateUserRequest(3),
				"Please enter a valid email address"
			};
			yield return new object[]
			{
				GetUpdateUserRequest(4),
				$"The first name length cannot exceed {User.MaxFirstNameLength} characters."
			};
			yield return new object[]
			{
				GetUpdateUserRequest(5),
				"Invalid user id"
			};
			yield return new object[]
			{
				GetUpdateUserRequest(6),
				"Invalid user id"
			};
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}

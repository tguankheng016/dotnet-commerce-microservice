using System.Collections;
using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.Application.Users.Dtos;
using CommerceMicro.IdentityService.Application.Users.Features.CreatingUser.V1;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.IdentityService.IntegrationTests.Utilities;
using CommerceMicro.Modules.Contracts;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using static Bogus.DataSets.Name;

namespace CommerceMicro.IdentityService.IntegrationTests.Users;

public class CreateUserTestBase : AppTestBase
{
	protected override string EndpointName { get; } = "user";

	protected CreateUserTestBase(
		ITestOutputHelper testOutputHelper,
		TestContainers testContainers
	) : base(testOutputHelper, testContainers)
	{
	}
}

public class CreateUser_Tests : CreateUserTestBase
{
	public CreateUser_Tests(
		ITestOutputHelper testOutputHelper,
		TestContainers testContainers
	) : base(testOutputHelper, testContainers)
	{
	}

	[Fact]
	public async Task Should_Create_User_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var totalCount = await DbContext.Users.CountAsync();
		var testUser = new Faker<CreateUserDto>()
			.RuleFor(x => x.Id, 0)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, f => f.Internet.UserName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);
		var request = testUser.Generate();

		await TestHarness.Start();

		// TODO: Soft Delete Violate Unique Index Of Username
		// Create a deleted user with the same username and email
		// await DbContext.Users.AddAsync(new User()
		// {
		// 	FirstName = request.FirstName!,
		// 	LastName = request.LastName!,
		// 	UserName = request.UserName,
		// 	NormalizedUserName = request.UserName!.ToUpper(),
		// 	Email = request.Email,
		// 	NormalizedEmail = request.Email!.ToUpper(),
		// 	IsDeleted = true
		// });

		// await DbContext.SaveChangesAsync();

		// Act
		var response = await client.PostAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var createResult = await response.Content.ReadFromJsonAsync<CreateUserResult>();
		createResult.Should().NotBeNull();
		createResult!.User.Should().NotBeNull();
		createResult!.User.Id.Should().BeGreaterThan(0);
		createResult!.User.UserName.Should().Be(request.UserName);
		createResult!.User.FirstName.Should().Be(request.FirstName);
		createResult!.User.LastName.Should().Be(request.LastName);
		createResult!.User.Email.Should().Be(request.Email);

		var newTotalCount = await DbContext.Users.CountAsync();
		newTotalCount.Should().Be(totalCount + 1);

		var publishedMessage = await TestHarness.Published.SelectAsync<UserCreatedEvent>().FirstOrDefault();
		publishedMessage.Should().NotBeNull();

		var userCreatedEvent = publishedMessage.Context.Message;
		userCreatedEvent.Id.Should().Be(createResult.User.Id);
		userCreatedEvent.UserName.Should().Be(createResult.User.UserName);
		userCreatedEvent.FirstName.Should().Be(createResult.User.FirstName);
		userCreatedEvent.LastName.Should().Be(createResult.User.LastName);

		await TestHarness.Stop();
	}

	[Theory]
	[InlineData(null, "admin@testgk.com", "Email 'admin@testgk.com' is already taken.")]
	[InlineData(UserConsts.DefaultUsername.User, null, "Username 'gkuser1' is already taken.")]
	public async Task Should_Not_Create_User_With_Duplicate_Username_Or_Email_Test(string? username, string? email, string errorMessage)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var testUser = new Faker<CreateUserDto>()
			.RuleFor(x => x.Id, 0)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, username)
			.RuleFor(x => x.Email, email)
			.RuleFor(x => x.Password, f => f.Internet.Password())
			.RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);

		await TestHarness.Start();

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
		var response = await client.PostAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse!.Detail.Should().Be(errorMessage);

		var publishedMessage = await TestHarness.Published.SelectAsync<UserCreatedEvent>().FirstOrDefault();
		publishedMessage.Should().BeNull();

		await TestHarness.Stop();
	}

	[Fact]
	public async Task Should_Create_User_With_Unauthorized_Error_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsUser();
		var testUser = new Faker<CreateUserDto>()
			.RuleFor(x => x.Id, 0)
			.RuleFor(u => u.FirstName, (f) => f.Name.FirstName(Gender.Male))
			.RuleFor(u => u.LastName, (f) => f.Name.LastName(Gender.Female))
			.RuleFor(x => x.UserName, f => f.Internet.UserName())
			.RuleFor(x => x.Email, f => f.Internet.Email())
			.RuleFor(x => x.Password, f => f.Internet.Password());
		var request = testUser.Generate();
		request.ConfirmPassword = request.Password;

		await TestHarness.Start();

		// Act
		var response = await client.PostAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();

		var publishedMessage = await TestHarness.Published.SelectAsync<UserCreatedEvent>().FirstOrDefault();
		publishedMessage.Should().BeNull();

		await TestHarness.Stop();
	}
}

public class CreateUserValidation_Tests : CreateUserTestBase
{
	public CreateUserValidation_Tests(
		ITestOutputHelper testOutputHelper,
		TestContainers testContainers
	) : base(testOutputHelper, testContainers)
	{
	}

	[Theory]
	[ClassData(typeof(GetValidateUserCreationTestData))]
	public async Task Should_Create_User_With_Invalid_Input_Test(CreateUserDto request, string errorMessage)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		await TestHarness.Start();

		// Act
		var response = await client.PostAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse!.Detail.Should().Be(errorMessage);

		var publishedMessage = await TestHarness.Published.SelectAsync<UserCreatedEvent>().FirstOrDefault();
		publishedMessage.Should().BeNull();

		await TestHarness.Stop();
	}

	private static CreateUserDto GetCreateUserRequest(int scenario)
	{
		var testUser = new Faker<CreateUserDto>()
			.RuleFor(x => x.Id, 0)
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
			case 1:
				{
					// Password Not Filled In
					testUser.RuleFor(u => u.Password, "");
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
					testUser.RuleFor(u => u.Id, 1);
					break;
				}
			case 6:
				{
					// Invalid Id
					testUser.RuleFor(u => u.Id, -5);
					break;
				}
		}

		return testUser.Generate();
	}

	private class GetValidateUserCreationTestData : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new object[]
			{
				GetCreateUserRequest(0),
				"Please enter the email address"
			};
			yield return new object[]
			{
				GetCreateUserRequest(1),
				"Please enter the password"
			};
			yield return new object[]
			{
				GetCreateUserRequest(2),
				"Passwords should match"
			};
			yield return new object[]
			{
				GetCreateUserRequest(3),
				"Please enter a valid email address"
			};
			yield return new object[]
			{
				GetCreateUserRequest(4),
				$"The first name length cannot exceed {User.MaxFirstNameLength} characters."
			};
			yield return new object[]
			{
				GetCreateUserRequest(5),
				"Invalid user id"
			};
			yield return new object[]
			{
				GetCreateUserRequest(6),
				"Invalid user id"
			};
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
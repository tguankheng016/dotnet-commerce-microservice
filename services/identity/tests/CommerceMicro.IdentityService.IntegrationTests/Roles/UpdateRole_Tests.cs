using System.Collections;
using CommerceMicro.IdentityService.Application.Roles.Constants;
using CommerceMicro.IdentityService.Application.Roles.Dtos;
using CommerceMicro.IdentityService.Application.Roles.Features.UpdatingRole.V1;
using CommerceMicro.IdentityService.IntegrationTests.Utilities;
using CommerceMicro.Modules.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CommerceMicro.IdentityService.IntegrationTests.Roles;

[Collection(RoleTestCollection1.Name)]
public class UpdateRoleTestBase : AppTestBase
{
	protected override string EndpointName { get; } = "role";

	protected UpdateRoleTestBase(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}
}

public class UpdateRole_Tests : UpdateRoleTestBase
{
	public UpdateRole_Tests(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}

	[Fact]
	public async Task Should_Update_Role_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var totalCount = await DbContext.Roles.CountAsync();
		var request = new EditRoleDto
		{
			Id = 2,
			Name = RoleConsts.RoleName.User,
			IsDefault = false
		};

		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var updateResult = await response.Content.ReadFromJsonAsync<UpdateRoleResult>();
		updateResult.Should().NotBeNull();
		updateResult!.Role.Should().NotBeNull();
		updateResult!.Role.IsDefault.Should().BeFalse();
		updateResult!.Role.Id.Should().Be(2);

		var newTotalCount = await DbContext.Roles.CountAsync();
		newTotalCount.Should().Be(totalCount);
	}

	[Theory]
	[ClassData(typeof(UpdateRoleErrorByInvalidRoleIdTestData))]
	public async Task Should_Get_Update_Role_Error_By_Invalid_RoleId_Test(long? roleId)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var request = new EditRoleDto
		{
			Id = roleId,
			Name = RoleConsts.RoleName.User,
			IsDefault = false
		};

		// Act	
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(roleId > 0 ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest);
	}

	[Fact]
	public async Task Should_Get_Update_Static_Role_Error_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var request = new EditRoleDto
		{
			Id = 1,
			Name = "TestRole"
		};

		// Act	
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
		failureResponse!.Detail.Should().Be("You cannot change the name of static role");
	}

	[Fact]
	public async Task Should_Update_Role_With_Unauthorized_Error_Test()
	{
		// Arrange
		var client = await ApiFactory.LoginAsUser();
		var request = new EditRoleDto
		{
			Id = 2,
			Name = "TestRole"
		};

		// Act
		var response = await client.PostAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

		var failureResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
		failureResponse.Should().NotBeNull();
	}

	public class UpdateRoleErrorByInvalidRoleIdTestData : TheoryData<long?>
	{
		public UpdateRoleErrorByInvalidRoleIdTestData()
		{
			Add(null);
			Add(0);
			Add(100);
		}
	}
}

public class UpdateRolePermissions_Tests : UpdateRoleTestBase
{
	public UpdateRolePermissions_Tests(
		ITestOutputHelper testOutputHelper,
		TestWebApplicationFactory webAppFactory
	) : base(testOutputHelper, webAppFactory)
	{
	}

	[Theory]
	[ClassData(typeof(GetUpdatedRolePermissionsTestData))]
	public async Task Should_Update_Role_Permissions_Test(long roleId, List<string> permissionsToUpdate, List<string> expectedGrantedPermissions, List<string> expectedProhibitedPermissions)
	{
		// Arrange
		var client = await ApiFactory.LoginAsAdmin();
		var role = await DbContext.Roles.FirstAsync(x => x.Id == roleId);
		var request = new EditRoleDto
		{
			Id = roleId,
			Name = role.Name!,
			GrantedPermissions = permissionsToUpdate
		};

		// Act
		var response = await client.PutAsJsonAsync(Endpoint, request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var expectedPermissions = await DbContext.UserRolePermissions
			.Where(x => x.RoleId == roleId && x.IsGranted).Select(x => x.Name).ToListAsync();
		expectedPermissions.Should().BeEquivalentTo(expectedGrantedPermissions);

		var prohibitedPermissions = await DbContext.UserRolePermissions
			.Where(x => x.RoleId == roleId && !x.IsGranted).Select(x => x.Name).ToListAsync();
		prohibitedPermissions.Should().BeEquivalentTo(expectedProhibitedPermissions);
	}

	private static List<string> GetPermissionsToUpdate(int scenario = 0)
	{
		switch (scenario)
		{
			case 0:
				{
					// Admin With Prohibited
					var allPermissions = AppPermissionProvider.GetPermissions().Select(x => x.Name).ToList();
					var permissionsToUpdate = allPermissions.Where(x => x != UserPermissions.Pages_Administration_Users).ToList();

					return permissionsToUpdate;
				}
			case 1:
				{
					// User Without Prohibited
					return new List<string>()
					{
						UserPermissions.Pages_Administration_Users,
						UserPermissions.Pages_Administration_Users_Create
					};
				}
		}

		return new List<string>();
	}

	private static List<string> GetExpectedPermissions(int scenario = 0)
	{
		switch (scenario)
		{
			case 0:
				{
					// Admin With Prohibited
					// return empty because admin is granted by default
					return new List<string>();
				}
			case 1:
				{
					// User Without Prohibited
					return new List<string>()
					{
						UserPermissions.Pages_Administration_Users,
						UserPermissions.Pages_Administration_Users_Create
					};
				}
		}

		return new List<string>();
	}

	private class GetUpdatedRolePermissionsTestData : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			// Admin With Prohibited
			yield return new object[]
			{
				1,
				GetPermissionsToUpdate(0),
				GetExpectedPermissions(0),
				new List<string>()
				{
					UserPermissions.Pages_Administration_Users
				}
			};
			// User Without Prohibited
			yield return new object[]
			{
				2,
				GetPermissionsToUpdate(1),
				GetExpectedPermissions(1),
				new List<string>()
			};
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}

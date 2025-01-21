using CommerceMicro.IdentityService.Application.Roles.Constants;
using CommerceMicro.IdentityService.Application.Roles.Models;
using CommerceMicro.IdentityService.Application.Users.Constants;
using CommerceMicro.IdentityService.Application.Users.Models;
using CommerceMicro.Modules.Core.Persistences;
using Microsoft.AspNetCore.Identity;

namespace CommerceMicro.IdentityService.Application.Data.Seed;

public class DataSeeder : IDataSeeder
{
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<Role> _roleManager;

	public DataSeeder(RoleManager<Role> roleManager, UserManager<User> userManager)
	{
		_roleManager = roleManager;
		_userManager = userManager;
	}

	public async Task SeedAllAsync()
	{
		await SeedRoles();
		await SeedUsers();
	}

	private async Task SeedRoles()
	{
		if (await _roleManager.RoleExistsAsync(RoleConsts.RoleName.Admin) == false)
		{
			await _roleManager.CreateAsync(new Role(RoleConsts.RoleName.Admin)
			{
				IsStatic = true
			});
		}

		if (await _roleManager.RoleExistsAsync(RoleConsts.RoleName.User) == false)
		{
			await _roleManager.CreateAsync(new Role(RoleConsts.RoleName.User)
			{
				IsStatic = true,
				IsDefault = true
			});
		}
	}

	private async Task SeedUsers()
	{
		// Seed Admin User
		if (await _userManager.FindByNameAsync(UserConsts.DefaultUsername.Admin) is null)
		{
			var adminUser = new User
			{
				FirstName = "Admin",
				LastName = "Tan",
				UserName = UserConsts.DefaultUsername.Admin,
				Email = "admin@testgk.com",
				SecurityStamp = Guid.NewGuid().ToString(),
				ExternalUserId = new Guid("c5fb6ddb-3551-487b-a38a-686d27376c30")
			};

			var result = await _userManager.CreateAsync(adminUser, "123qwe");

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(adminUser, RoleConsts.RoleName.Admin);
			}

			var normalUser = new User
			{
				FirstName = "User",
				LastName = "Tan",
				UserName = UserConsts.DefaultUsername.User,
				Email = "user@testgk.com",
				SecurityStamp = Guid.NewGuid().ToString()
			};

			result = await _userManager.CreateAsync(normalUser, "123qwe");

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(normalUser, RoleConsts.RoleName.User);
			}
		}
	}
}

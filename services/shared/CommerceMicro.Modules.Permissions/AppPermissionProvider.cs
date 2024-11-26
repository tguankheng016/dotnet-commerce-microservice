namespace CommerceMicro.Modules.Permissions;

public static class UserPermissions
{
	public const string GroupName = "Users";

	public const string Pages_Administration_Users = "Pages.Administration.Users";
	public const string Pages_Administration_Users_Create = "Pages.Administration.Users.Create";
	public const string Pages_Administration_Users_Edit = "Pages.Administration.Users.Edit";
	public const string Pages_Administration_Users_Delete = "Pages.Administration.Users.Delete";
	public const string Pages_Administration_Users_ChangePermissions = "Pages.Administration.Users.ChangePermissions";
}

public static class RolePermissions
{
	public const string GroupName = "Roles";

	public const string Pages_Administration_Roles = "Pages.Administration.Roles";
	public const string Pages_Administration_Roles_Create = "Pages.Administration.Roles.Create";
	public const string Pages_Administration_Roles_Edit = "Pages.Administration.Roles.Edit";
	public const string Pages_Administration_Roles_Delete = "Pages.Administration.Roles.Delete";
}

public static class CategoryPermissions
{
	public const string GroupName = "Categories";

	public const string Pages_Categories = "Pages.Categories";
	public const string Pages_Categories_Create = "Pages.Categories.Create";
	public const string Pages_Categories_Edit = "Pages.Categories.Edit";
	public const string Pages_Categories_Delete = "Pages.Categories.Delete";
	public const string Pages_Categories_View = "Pages.Categories.View";
}

public static class ProductPermissions
{
	public const string GroupName = "Products";

	public const string Pages_Products = "Pages.Products";
	public const string Pages_Products_Create = "Pages.Products.Create";
	public const string Pages_Products_Edit = "Pages.Products.Edit";
	public const string Pages_Products_Delete = "Pages.Products.Delete";
	public const string Pages_Products_View = "Pages.Products.View";
}

public static class AppPermissionProvider
{
	public static List<Permission> GetPermissions()
	{
		var permissions = new List<Permission>
		{
			new Permission(RolePermissions.Pages_Administration_Roles, "View roles", RolePermissions.GroupName),
			new Permission(RolePermissions.Pages_Administration_Roles_Create, "Create role", RolePermissions.GroupName),
			new Permission(RolePermissions.Pages_Administration_Roles_Edit, "Edit role", RolePermissions.GroupName),
			new Permission(RolePermissions.Pages_Administration_Roles_Delete, "Delete role", RolePermissions.GroupName),

			new Permission(UserPermissions.Pages_Administration_Users, "View users", UserPermissions.GroupName),
			new Permission(UserPermissions.Pages_Administration_Users_Create, "Create user", UserPermissions.GroupName),
			new Permission(UserPermissions.Pages_Administration_Users_Edit, "Edit user", UserPermissions.GroupName),
			new Permission(UserPermissions.Pages_Administration_Users_Delete, "Delete user", UserPermissions.GroupName),
			new Permission(UserPermissions.Pages_Administration_Users_ChangePermissions, "Change user permissions", UserPermissions.GroupName),

			new Permission(CategoryPermissions.Pages_Categories, "View categories", CategoryPermissions.GroupName),
			new Permission(CategoryPermissions.Pages_Categories_Create, "Create category", CategoryPermissions.GroupName),
			new Permission(CategoryPermissions.Pages_Categories_Edit, "Edit category", CategoryPermissions.GroupName),
			new Permission(CategoryPermissions.Pages_Categories_Delete, "Delete category", CategoryPermissions.GroupName),
			new Permission(CategoryPermissions.Pages_Categories_View, "View category details", CategoryPermissions.GroupName),

			new Permission(ProductPermissions.Pages_Products, "View products", ProductPermissions.GroupName),
			new Permission(ProductPermissions.Pages_Products_Create, "Create product", ProductPermissions.GroupName),
			new Permission(ProductPermissions.Pages_Products_Edit, "Edit product", ProductPermissions.GroupName),
			new Permission(ProductPermissions.Pages_Products_Delete, "Delete product", ProductPermissions.GroupName),
			new Permission(ProductPermissions.Pages_Products_View, "View product details", ProductPermissions.GroupName),
		};

		return permissions;
	}
}
using CommerceMicro.PermissionService;

namespace CommerceMicro.Modules.Permissions;

public class DefaultPermissionDbManager : IPermissionDbManager
{
	private readonly PermissionGrpcService.PermissionGrpcServiceClient _permissionGrpcServiceClient;

	public DefaultPermissionDbManager(
			PermissionGrpcService.PermissionGrpcServiceClient permissionGrpcServiceClient
		)
	{
		_permissionGrpcServiceClient = permissionGrpcServiceClient;
	}

	public async Task<Dictionary<string, string>> GetGrantedPermissionsAsync(long userId, CancellationToken cancellationToken = default)
	{
		var response = await _permissionGrpcServiceClient.GetUserGrantedPermissionsAsync(new GetUserPermissionsRequest()
		{
			UserId = userId
		});

		var permissionsList = response.Permissions.ToList();

		return permissionsList.ToDictionary(x => x, x => x);
	}
}

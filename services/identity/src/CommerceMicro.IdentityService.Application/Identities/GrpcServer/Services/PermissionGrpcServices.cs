using CommerceMicro.Modules.Permissions;
using CommerceMicro.PermissionService;
using Grpc.Core;

namespace CommerceMicro.IdentityService.Application.Identities.GrpcServer.Services;

public class PermissionGrpcServices : PermissionGrpcService.PermissionGrpcServiceBase
{
    private readonly IPermissionDbManager _permissionDbManager;

    public PermissionGrpcServices(
        IPermissionDbManager permissionDbManager)
    {
        _permissionDbManager = permissionDbManager;
    }

    public override async Task<GetUserPermissionsResponse> GetUserGrantedPermissions(GetUserPermissionsRequest request, ServerCallContext context)
    {
        var grantedPermissions = await _permissionDbManager
            .GetGrantedPermissionsAsync(request.UserId, context.CancellationToken);

        var response = new GetUserPermissionsResponse();

        foreach (var permission in grantedPermissions)
        {
            response.Permissions.Add(permission.Key);
        }

        return response;
    }
}

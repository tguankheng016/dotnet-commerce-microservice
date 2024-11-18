namespace CommerceMicro.Modules.Permissions;

public class DefaultPermissionDbManager : IPermissionDbManager
{
    public Task<Dictionary<string, string>> GetGrantedPermissionsAsync(long userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

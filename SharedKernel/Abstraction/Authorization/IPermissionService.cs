namespace Integral.App.Abstraction.Authorization;

public interface IPermissionService
{
    public Task<bool> HasPermissionAsync(string permission);
    public void InvalidateRoleCache(string roleCode);
}
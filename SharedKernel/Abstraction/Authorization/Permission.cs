namespace Integral.App.Abstraction.Authorization;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequiresPermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}
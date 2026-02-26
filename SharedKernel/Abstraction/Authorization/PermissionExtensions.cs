namespace Integral.App.Abstraction.Authorization;

public static class PermissionExtensions
{
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permission)
    {
        builder.AddEndpointFilterFactory((context, next) => new PermissionFilter(permission, next).InvokeAsync);
        builder.WithMetadata(new RequiresPermissionAttribute(permission));
        
        return builder;
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Integral.App.Abstraction.Authorization;

public sealed class PermissionFilter(string permission, EndpointFilterDelegate next)
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context)
    {
        var http = context.HttpContext;
        var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();
        
        var permissionService = http.RequestServices.GetRequiredService<IPermissionService>();
        var authorized = await permissionService.HasPermissionAsync(permission);
        
        if (!authorized) 
            return Results.Unauthorized();

        return await next(context);
    }
}
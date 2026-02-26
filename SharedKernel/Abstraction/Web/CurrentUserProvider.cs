using System.Security.Claims;

namespace SharedKernel.Abstraction.Web;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
{
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string GetUsername()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            return "";

        var username = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return username ?? "";
    }

    public string? GetDisplayName()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext?.User.FindFirstValue("name");
    }
    
    public string? GetRole()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext?.User.FindFirstValue(ClaimTypes.Role);
    }
}
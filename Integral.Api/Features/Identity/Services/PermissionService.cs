using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Models;
using Integral.App.Abstraction.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Services;

public class PermissionService(
    PrintingDbContext dbContext,
    CurrentUserProvider userProvider,
    IMemoryCache cache
) : IPermissionService
{
    private const string CachePrefix = "permissions:";

    public async Task<bool> HasPermissionAsync(string permission)
    {
        var roleCode = userProvider.GetRole()?.ToLower();

        if (cache.TryGetValue<HashSet<string>>($"{CachePrefix}{roleCode?.ToLower()}", out var permissions))
            return HasWildcardPermission(permission.ToLowerInvariant(), permissions);
        
        var role = await dbContext.Roles
            .Include(r => r.Claims)
            .FirstOrDefaultAsync(r => r.Code == roleCode);

        if (role == null) return false;

        permissions = role.Claims
            .Where(c => c.Type == AppClaimTypes.Permission)
            .Select(c => c.Value.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        cache.Set(CachePrefix + roleCode, permissions, TimeSpan.FromMinutes(10));

        // return HasWildcardPermission(permission.ToLowerInvariant(), permissions);

        return permissions.Any(permission.EqualsWildcard);
    }
    
    public void InvalidateRoleCache(string roleCode)
    {
        cache.Remove($"{CachePrefix}{roleCode.ToLower()}");
    }

    private static bool HasWildcardPermission(string required, HashSet<string> available)
    {
        if (available.Contains(required) || available.Contains("*")) 
            return true;

        var requiredParts = required.Split('.');
    
        foreach (var permission in available)
        {
            if (!permission.Contains('*')) continue;
        
            var permParts = permission.Split('.');
        
            if (!MatchesPattern(requiredParts, permParts))
                continue;
            
            return true;
        }

        return false;
    }

    private static bool MatchesPattern(string[] required, string[] pattern)
    {
        int ri = 0, pi = 0;
        int star = -1, match = 0;

        while (ri < required.Length)
        {
            if (pi < pattern.Length &&
                string.Equals(required[ri], pattern[pi], StringComparison.OrdinalIgnoreCase))
            {
                ri++;
                pi++;
            }
            else if (pi < pattern.Length && pattern[pi] == "*")
            {
                star = pi;
                match = ri;
                pi++;
            }
            else if (star != -1)
            {
                pi = star + 1;
                match++;
                ri = match;
            }
            else
            {
                return false;
            }
        }

        while (pi < pattern.Length && pattern[pi] == "*")
            pi++;

        return pi == pattern.Length;
    }
}
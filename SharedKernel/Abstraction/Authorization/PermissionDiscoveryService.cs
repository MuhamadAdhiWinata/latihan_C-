namespace Integral.App.Abstraction.Authorization;

public class PermissionDiscoveryService(EndpointDataSource endpointDataSource)
{
    private readonly HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);
    private bool _scanned = false;

    public IReadOnlyCollection<string> DiscoverAllPermissions()
    {
        if (_scanned)
            return _permissions;

        foreach (var endpoint in endpointDataSource.Endpoints.OfType<RouteEndpoint>())
        {
            foreach (var attr in endpoint.Metadata.OfType<RequiresPermissionAttribute>())
            {
                if (!string.IsNullOrWhiteSpace(attr.Permission))
                    _permissions.Add(attr.Permission);
            }
        }

        _scanned = true;
        return _permissions;
    }

    public IReadOnlyCollection<string> GetCachedPermissions() => _permissions;
}
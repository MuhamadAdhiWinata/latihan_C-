using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record FindAllPermissions : IQuery<FindAllPermissionsResult>
{
}

public record FindAllPermissionsResult(string[] Data)
{
}

public class FindAllPermissionHandler(PermissionDiscoveryService permissionDiscoveryService)
    : IQueryHandler<FindAllPermissions, FindAllPermissionsResult>
{
    public async Task<FindAllPermissionsResult> Handle(FindAllPermissions request, CancellationToken cancellationToken)
    {
        return new FindAllPermissionsResult(permissionDiscoveryService.DiscoverAllPermissions().ToArray());
    }
}

public class FindAllPermissionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{IdentityRoot.ApiPath}/permissions", Handle)
            .RequireAuthorization()
            .WithTags("Identity Permission")
            .WithName("Find All Permissions");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllPermissions request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
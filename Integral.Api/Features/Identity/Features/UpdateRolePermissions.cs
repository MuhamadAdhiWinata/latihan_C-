using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Models;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record UpdateRolePermissions(string Code, string[] Permissions) : ICommand<UpdateRolePermissionsResult>
{
}

public record UpdateRolePermissionsResult
{
}

public class UpdateRolePermissionHandler(PrintingDbContext dbContext, IPermissionService permissionService)
    : ICommandHandler<UpdateRolePermissions, UpdateRolePermissionsResult>
{
    public async Task<UpdateRolePermissionsResult> Handle(UpdateRolePermissions request,
        CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles
            .Include(x => x.Claims)
            .Where(x => x.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (role == null)
            throw new InvalidOperationException($"Role with code {request.Code} not found");

        var permissionClaims = role.Claims.Where(x => x.Type == AppClaimTypes.Permission).ToList();
        foreach (var claim in permissionClaims) role.Claims.Remove(claim);

        foreach (var permission in request.Permissions)
            role.Claims.Add(new RoleClaim
            {
                RoleId = role.Id,
                Type = AppClaimTypes.Permission,
                Value = permission
            });

        permissionService.InvalidateRoleCache(request.Code);

        return new UpdateRolePermissionsResult();
    }
}

public class UpdateRolePermissionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{IdentityRoot.ApiPath}/roles/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Identity Roles")
            .WithName("Update Roles");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] UpdateRolePermissionsDto request,
        string code)
    {
        var command = new UpdateRolePermissions(code, request.Permissions);
        var res = await mediator.Send(command);

        return Results.Ok(res);
    }

    private record UpdateRolePermissionsDto(string[] Permissions);
}
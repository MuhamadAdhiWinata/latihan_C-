using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record FindAllRoles : IQuery<FindAllRolesResult>
{
}

public record FindAllRolesResult(RoleDto[] Roles);

public class FindAllRolesHandler(PrintingDbContext dbContext) : IQueryHandler<FindAllRoles, FindAllRolesResult>
{
    public async Task<FindAllRolesResult> Handle(FindAllRoles request, CancellationToken cancellationToken)
    {
        var roles = await dbContext.Roles
            .Include(x => x.Claims)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new FindAllRolesResult(roles.ToArray());
    }
}

public class FindAllRolesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{IdentityRoot.ApiPath}/roles", Handle)
            .RequireAuthorization()
            .WithTags("Identity Roles")
            .WithName("Find All Roles");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllRoles request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
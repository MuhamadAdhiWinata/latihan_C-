using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Dtos;
using Integral.Api.Features.Inventories.InventoryMutations.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record RoleByCodeDto(
    long Id,
    string Code,
    string Name,
    string[] Permissions);

public record FindByCodeRoleResult(RoleByCodeDto Data);

public record FindByCodeRole(string Code)  : IQuery<FindByCodeRoleResult>
{
}

public class FindByCodeRoleHandler(PrintingDbContext dbContext) : IQueryHandler<FindByCodeRole, FindByCodeRoleResult>
{
    public async Task<FindByCodeRoleResult> Handle(FindByCodeRole request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles
            .Include(x => x.Claims)
            .Where(x => x.Code == request.Code)
            .Select(x => new RoleByCodeDto(x.Id, x.Code, x.Name,
                x.Claims.Where(y => y.Type == "permission")
                    .Select(y => y.Value)
                    .ToArray()))
            .FirstOrDefaultAsync(cancellationToken);

        if (role == null) throw new BarangNotFoundException("");

        return new FindByCodeRoleResult(role);
    }
}

public class FindByCodeRoleEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{IdentityRoot.ApiPath}/roles/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Identity Roles")
            .WithName("Find By Code Roles");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeRole(code);
        
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
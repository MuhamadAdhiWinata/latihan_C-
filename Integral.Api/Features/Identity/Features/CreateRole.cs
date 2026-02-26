using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Dtos;
using Integral.Api.Features.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record CreateRole(string Code, string Name, string Description = "") : ICommand<CreateRoleResult>
{
}

public record CreateRoleResult(RoleDto Data)
{
}

public class CreateRoleHandler(PrintingDbContext dbContext) : ICommandHandler<CreateRole, CreateRoleResult>
{
    public async Task<CreateRoleResult> Handle(CreateRole request, CancellationToken cancellationToken)
    {
        var role = new Role()
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description
        };

        await dbContext.Roles.AddAsync(role, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateRoleResult(role.ToDto());
    }
}

public class CreateRoleEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{IdentityRoot.ApiPath}/roles", Handle)
            .RequireAuthorization()
            .WithTags("Identity Roles")
            .WithName("Create Role");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateRole request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
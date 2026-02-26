using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record ModifyUser(string Username, string? Name, string? RoleCode) : ICommand<ModifyUserResult>
{
}

public record ModifyUserResult()
{
}

public class ModifyUserHandler(PrintingDbContext dbContext) : ICommandHandler<ModifyUser, ModifyUserResult>
{
    public async Task<ModifyUserResult> Handle(ModifyUser request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.Where(x => x.Code == request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null) throw new Exception("User not found");

        user.Name = request.Name ?? user.Name;
        user.GroupCode = request.RoleCode ?? user.GroupCode;

        return new ModifyUserResult();
    }
}

public class ModifyUserEndpoint : IMinimalEndpoint
{
    record ModifyUserBody(string? Name, string? RoleCode);

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{IdentityRoot.ApiPath}/users/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Identity Users")
            .WithName("Modify User");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] ModifyUserBody body,
        string code)
    {
        var request = new ModifyUser(code, body.Name, body.RoleCode);

        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
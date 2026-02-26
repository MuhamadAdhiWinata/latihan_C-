using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record FindAllUsersResult(UserDto[] Data);

public record FindAllUsers() : IQuery<FindAllUsersResult>;

public class FindAllUsersHandler(PrintingDbContext dbContext) : IQueryHandler<FindAllUsers, FindAllUsersResult>
{
    public async Task<FindAllUsersResult> Handle(FindAllUsers request, CancellationToken cancellationToken)
    {
        var users = await dbContext.Users.AsNoTracking().Select(x => x.ToDto()).ToListAsync(cancellationToken);
        return new FindAllUsersResult(users.ToArray());
    }
}

public class FindAllUsersEndpoint() : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{IdentityRoot.ApiPath}/users", Handle)
            .RequireAuthorization()
            .WithTags("Identity Users")
            .WithName("Find All Users");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllUsers request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record CreateUser(string Username, string Name, string Password) : ICommand<CreateUserResult>;

public record CreateUserResult(string Username);

public class CreateUserHandler(PrintingDbContext dbContext) : ICommandHandler<CreateUser, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        var existingUsername = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Code == request.Username)
            .AnyAsync(cancellationToken);

        if (existingUsername) throw new Exception("Username already exists");

        var user = new User()
        {
            BranchCode = AppDefaults.BranchCode,
            Code = request.Username,
            Name = request.Name,
            GroupCode = "",
            CreatedBy = "SYSTEM",
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now,
        };

        user.SetPassword(request.Password);

        await dbContext.AddAsync(user, cancellationToken);

        return new CreateUserResult(user.Code);
    }
}

public class CreateUserEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{IdentityRoot.ApiPath}/users", Handle)
            .RequireAuthorization()
            .WithTags("Identity Users")
            .WithName("Create User");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateUser request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
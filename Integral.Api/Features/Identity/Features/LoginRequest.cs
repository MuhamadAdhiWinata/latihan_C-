using System.Security.Claims;
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Identity.Features;

public record LoginRequest(string Username, string Password) : ICommand<LoginRequestResult>;

public record LoginRequestResult(string AccessToken, string Name, string[] Permissions);

public class LoginRequestHandler(PrintingDbContext dbContext, JwtProvider jwtProvider)
    : ICommandHandler<LoginRequest, LoginRequestResult>
{
    public async Task<LoginRequestResult> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Code == request.Username, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User {request.Username} not found.");

        if (!user.VerifyPassword(request.Password))
            throw new InvalidOperationException("Invalid password.");

        var claims = new List<Claim>
        {
            new("sub", user.Code),
            new("name", user.Name),
            new(ClaimTypes.Role, user.GroupCode)
        };

        var accessToken = jwtProvider.GenerateToken(claims);

        var role = await dbContext.Roles
            .Include(x => x.Claims)
            .Where(x => x.Code == user.GroupCode)
            .FirstOrDefaultAsync(cancellationToken);

         var permissions = role != null ? 
             role.Claims.Where(x => x.Type == "permission").Select(x => x.Value) 
             : Array.Empty<string>();
        

        return new LoginRequestResult(accessToken, user.Name, permissions.ToArray());
    }
}

public class LoginRequestEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{IdentityRoot.AuthApiPath}/login", Handle)
            .WithTags("Auth")
            .WithName("Login");

        return builder;
    }

    private async Task<IResult> Handle([FromServices] IMediator mediator, [FromBody] LoginRequest request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
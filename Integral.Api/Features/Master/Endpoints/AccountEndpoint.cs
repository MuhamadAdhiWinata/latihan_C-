using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Endpoints;

public class AccountEndpoint : IMinimalEndpoint
{
    private const string ApiPath = $"{MasterRoot.BaseApiPath}/accounts";
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup($"{ApiPath}").WithTags($"Master Accounts").RequireAuthorization();

        group.MapGet("", async ([FromServices] PrintingDbContext dbContext) =>
        {
            var res = await dbContext.Accounts
                .Select(x => x.ToDto())
                .ToListAsync();

            return Results.Ok(new { Data = res });
        });


        group.MapGet("{code}", async ([FromServices] PrintingDbContext dbContext, string code) =>
        {
            var res = await dbContext.Accounts
                .Where(x => x.Code == code)
                .Select(x => x.ToDto())
                .FirstOrDefaultAsync();

            return Results.Ok(new { Data = res });
        });

        return builder;
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryIns.Features;

public record FindAllInventoryInResult(InventoryInHeaderDto[] Data);

public record FindAllInventoryIn(int Limit = 100, int Offset = 0, string Search = "")
    : IQuery<FindAllInventoryInResult>;

public class FindAllInventoryInHandler(PrintingDbContext printingDb)
    : IQueryHandler<FindAllInventoryIn, FindAllInventoryInResult>
{
    public async Task<FindAllInventoryInResult> Handle(FindAllInventoryIn request, CancellationToken cancellationToken)
    {
        var results = await printingDb.F303s
            .OrderByDescending(f => f.Iinno)
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new FindAllInventoryInResult(results.ToArray());
    }
}

public class FindAllInventoryInEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/inventory/in", Handle)
            .RequireAuthorization()
            .WithTags("Inventory In")
            .WithName("Find All Inventory In");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllInventoryIn request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
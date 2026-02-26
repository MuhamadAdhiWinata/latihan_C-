using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryOuts.Queries;

public record GetAllInventoryOutResult(int Count, InventoryOutHeaderDto[] Data);

public record GetAllInventoryOut(
    int Limit = 100,
    int Offset = 0,
    string Search = ""
) : IQuery<GetAllInventoryOutResult>;

public class GetAllInventoryOutHandler(PrintingDbContext printingDb)
    : IQueryHandler<GetAllInventoryOut, GetAllInventoryOutResult>
{
    public async Task<GetAllInventoryOutResult> Handle(GetAllInventoryOut request, CancellationToken cancellationToken)
    {
        var baseQuery = printingDb.InventoryOuts
            .AsNoTracking()
            .OrderByDescending(i => i.Iotno)
            .Where(o =>
                o.Iotno.Contains(request.Search)
            );

        if (baseQuery == null) throw new AppException("Inventory Outs is null");

        var count = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Select(o => o.ToDto())
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new GetAllInventoryOutResult(count, items.ToArray());
    }
}

public class GetALlInventoryOutsController : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] GetAllInventoryOut request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryOutRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{InventoryOutRoot.Title}")
            .WithName($"Find All {InventoryOutRoot.Title}");

        return builder;
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.Queries;

public record ItemDto(
    string Code,
    string Name,
    string UnitCode,
    decimal Price,
    string? Sku,
    string ItemTypeCode,
    string ItemTypeName,
    short ActiveStatus,
    string MerkCode,
    decimal Stock
);

public record FindAllItemResult(int Count, ItemDto[] Data);

public record FindAllItem(
    int Limit = 100,
    int Offset = 0,
    string Search = "",
    bool? IsProduct = null,
    bool? IsAvailable = null
) : IQuery<FindAllItemResult>;

public class FindAllItemHandler(PrintingDbContext printingDb) : IQueryHandler<FindAllItem, FindAllItemResult>
{
    public async Task<FindAllItemResult> Handle(FindAllItem request, CancellationToken cancellationToken)
    {
        var baseQuery = printingDb.Items
            .AsNoTracking()
            .Include(i => i.ItemType)
            .OrderBy(i => i.Code)
            .GroupJoin(
                printingDb.StockBatches,
                i => i.Code,
                s => s.ItemCode,
                (item, stocks) => new { item, stocks }
            )
            .Where(o =>
                o.item.Code.Contains(request.Search) ||
                o.item.Name.Contains(request.Search));
        
        if (request.IsProduct.HasValue)
            baseQuery = baseQuery.Where(o => string.IsNullOrWhiteSpace(o.item.Sku) != request.IsProduct.Value);
        
        if (request.IsAvailable.HasValue)
            baseQuery = baseQuery.Where(o => o.stocks.Sum(s => s.ActualStock - s.UsedStock) > 0 == request.IsAvailable.Value);
        
        var count = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Select(o => new ItemDto(
                o.item.Code,
                o.item.Name,
                o.item.UnitCode,
                o.item.Price,
                o.item.Sku,
                o.item.ItemTypeCode,
                o.item.ItemType.Name,
                o.item.ActiveStatus,
                o.item.MerkCode,
                o.stocks.Sum(s => s.ActualStock - s.UsedStock)
            ))
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new FindAllItemResult(count, items.ToArray());
    }
}

public class FindAllItemController : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllItem request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/items", Handle)
            .RequireAuthorization()
            .WithTags("DEPRECATED")
            .WithName("Find All Items");
        
        return builder;
    }

}
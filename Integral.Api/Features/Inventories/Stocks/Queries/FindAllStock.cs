using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using Integral.Api.Features.Master.Items.Features.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.Queries;

public record FindAllStock(
    int Limit = 100,
    int Offset = 0,
    string Search = "",
    bool? HasStock = null,
    bool? HasSku = null,
    string ItemCode = "",
    OrderDirection? Order = null
) : IQuery<FindAllStockResult>;

public record FindAllStockResult(int Count, StockDto[] Data);

public record StockDto(
    string ItemCode,
    string ItemName,
    string UnitCode,
    string? Sku,
    string ItemTypeCode,
    string ItemTypeName,
    decimal Stock,
    StockAggregateDto[] Details
);

public record StockAggregateDto(
    string WarehouseCode,
    decimal Cost,
    decimal Stock
);

public class FindAllStockHandler(PrintingDbContext printingDb)
    : IQueryHandler<FindAllStock, FindAllStockResult>
{
    public async Task<FindAllStockResult> Handle(FindAllStock request, CancellationToken cancellationToken)
    {
        var baseQuery = printingDb.StockBatches
            .Include(x => x.Item)
            .ThenInclude(i => i.ItemType)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.ItemCode))
        {
            baseQuery = baseQuery.Where(x => x.ItemCode == request.ItemCode);
        }

        var groupedQuery = baseQuery
            .GroupBy(b => new
            {
                b.ItemCode,
                ItemName = b.Item.Name,
                b.Item.UnitCode,
                b.Item.Sku,
                b.Item.ItemTypeCode,
                ItemTypeName = b.Item.ItemType.Name,
                b.Item.CreatedDate
            })
            .Select(g => new
            {
                g.Key.ItemCode,
                g.Key.ItemName,
                g.Key.UnitCode,
                g.Key.Sku,
                g.Key.ItemTypeCode,
                g.Key.ItemTypeName,
                TotalAvailable = g.Sum(x => x.ActualStock - x.UsedStock),
                g.Key.CreatedDate
            });
        switch (request.Order)
        {
            case OrderDirection.Ascending:
                groupedQuery = groupedQuery.OrderBy(x => x.CreatedDate);
                break;

            case OrderDirection.Descending:
                groupedQuery = groupedQuery.OrderByDescending(x => x.CreatedDate);
                break;
        }

        var grouped = await groupedQuery
            .Where(x => x.ItemCode.Contains(request.Search) || x.ItemName.Contains(request.Search))
            .Where(x => !request.HasSku.HasValue || (x.Sku != null) == request.HasSku)
            .Where(x => !request.HasStock.HasValue || (x.TotalAvailable > 0) == request.HasStock)
            .ToListAsync(cancellationToken);

        var totalCount = grouped.Count;

        var pagedItems = grouped
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToList();

        var itemCodes = pagedItems.Select(x => x.ItemCode).ToArray();

        var batches = await printingDb.StockBatches
            .AsNoTracking()
            .Where(b => itemCodes.AsEnumerable().Contains(b.ItemCode))
            .ToListAsync(cancellationToken);

        var result = pagedItems.Select(item =>
            {
                var itemBatches = batches.Where(b => b.ItemCode == item.ItemCode);

                var warehouseGroups = itemBatches
                    .GroupBy(b => b.WarehouseCode)
                    .Select(g => new StockAggregateDto(
                        WarehouseCode: g.Key,
                        Cost: g.Sum(x => x.Cogsidr),
                        Stock: g.Sum(x => x.ActualStock - x.UsedStock)
                    ))
                    .ToArray();

                var totalStock = warehouseGroups.Sum(w => w.Stock);

                return new StockDto(
                    item.ItemCode,
                    item.ItemName ?? "-",
                    item.UnitCode ?? "-",
                    item.Sku,
                    item.ItemTypeCode ?? "-",
                    item.ItemTypeName ?? "-",
                    totalStock,
                    warehouseGroups
                );
            })
            .ToArray();

        return new FindAllStockResult(totalCount, result);
    }
}

public class FindAllStockEndpoint : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllStock request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/inventory/stock", Handle)
            .RequireAuthorization()
            .WithTags("Stock")
            .WithName("Find All Inventory Stock");

        return builder;
    }
}
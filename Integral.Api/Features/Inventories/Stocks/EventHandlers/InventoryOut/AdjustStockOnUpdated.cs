using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Entities;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.InventoryOut;

public class AdjustStockOnUpdated(PrintingDbContext dbContext, CurrentUserProvider currentUser) : INotificationHandler<InventoryOutUpdated>
{
    public async Task Handle(InventoryOutUpdated notification, CancellationToken cancellationToken)
    {
        var entries = await dbContext.StockTransactions
            .Include(x => x.ReversedBy)
            .Include(x => x.StockBatch)
            .Where(x => x.ReverseOfId == null)
            .Where(x => x.ReversedBy == null)
            .Where(x => x.RefCode == notification.Code)
            .ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            var aggregate = new StockAggregate([entry.StockBatch]);
            aggregate.Reverse(entry, currentUser.GetUsername());
        }
        
        foreach (var item in notification.Data.F306s)
        {
            var batches = await dbContext.StockBatches
                .Where(x => x.ItemCode == item.ItemCode)
                .Where(x => x.WarehouseCode == notification.Data.WareHouseCode)
                .ToListAsync(cancellationToken);

            var aggregate = new StockAggregate(batches.ToArray());
            aggregate.Consume(item.ItemCode, notification.Data.WareHouseCode, item.Quantity, currentUser.GetUsername(), notification.Code);
        }
    }


}
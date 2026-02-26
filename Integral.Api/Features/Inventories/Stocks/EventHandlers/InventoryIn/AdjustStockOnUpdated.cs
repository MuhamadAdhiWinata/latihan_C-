using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Events;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.InventoryIn;

public class AdjustStockOnUpdated(PrintingDbContext dbContext) : INotificationHandler<InventoryInUpdated>
{
    public async Task Handle(InventoryInUpdated notification, CancellationToken cancellationToken)
    {
        var batches = await dbContext.StockBatches
            .Where(x => x.Available > 0)
            .ToListAsync(cancellationToken);
        
        var stock = new StockAggregate(batches.ToArray());
        
    }
}
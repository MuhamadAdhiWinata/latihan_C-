using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Entities;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.InventoryOut;

public class AdjustStockOnDeleted(PrintingDbContext dbContext,CurrentUserProvider currentUserProvider):INotificationHandler<InventoryOutDeleted>
{
    public async Task Handle(InventoryOutDeleted notification, CancellationToken cancellationToken)
    {
        var entries = await dbContext.StockTransactions
            .Include(x => x.StockBatch)
            .Where(x => x.ReverseOfId == null)
            .Where(x => x.RefCode == notification.Code)
            .ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            var aggregate = new StockAggregate([entry.StockBatch]);
            aggregate.Reverse(entry, currentUserProvider.GetUsername());
        }
    }
}
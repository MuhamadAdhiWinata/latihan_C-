using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Entities;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.InventoryOut;

public class RemoveStockOnCreated(PrintingDbContext printingDb,CurrentUserProvider currentUser) : INotificationHandler<InventoryOutCreated>
{
    public async Task Handle(InventoryOutCreated notification, CancellationToken cancellationToken)
    { 
        foreach (var item in notification.Data.F306s)
        {
            var batches = await printingDb.StockBatches
                .Where(x => x.ItemCode == item.ItemCode)
                .Where(x => x.WarehouseCode == notification.Data.WareHouseCode)
                .ToListAsync(cancellationToken);

            var aggregate = new StockAggregate(batches.ToArray());
            aggregate.Consume(item.ItemCode, notification.Data.WareHouseCode, item.Quantity, currentUser.GetUsername(), notification.Code);
        }
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.InventoryMutations;

public class RemoveStockOnCreated(PrintingDbContext dbContext, CurrentUserProvider currentUser) : INotificationHandler<MutationCreated>
{
    public async Task Handle(MutationCreated notification, CancellationToken cancellationToken)
    {
        foreach (var item in notification.Data.MutationItems)
        {
            var batches = await dbContext.StockBatches
                .Where(x => x.ItemCode == item.ItemCode)
                .Where(x => x.WarehouseCode == notification.Data.WarehouseCode)
                .ToListAsync(cancellationToken);

            var aggregate = new StockAggregate(batches.ToArray());
            aggregate.Consume(item.ItemCode, notification.Data.WarehouseCode, item.Quantity, currentUser.GetUsername(), notification.Code);
        }
    }
}
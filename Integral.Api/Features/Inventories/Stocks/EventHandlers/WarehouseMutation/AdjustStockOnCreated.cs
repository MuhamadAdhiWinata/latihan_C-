using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.Stocks.Models;
using Integral.Api.Features.Inventories.WarehouseMutations.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.WarehouseMutation;

public class AdjustStockOnCreated(PrintingDbContext dbContext, CurrentUserProvider currentUser)
    : INotificationHandler<WarehouseMutationCreated>
{
    public async Task Handle(WarehouseMutationCreated notification, CancellationToken cancellationToken)
    {
        foreach (var item in notification.Data.Items)
        {
            var batches = await dbContext.StockBatches
                .Where(x => x.ItemCode == item.ItemCode)
                .Where(x => x.WarehouseCode == notification.Data.WareHouseSourceCode)
                .ToListAsync(cancellationToken);

            var aggregate = new StockAggregate(batches.ToArray());
            aggregate.Consume(item.ItemCode, notification.Data.WareHouseSourceCode, item.Quantity,
                currentUser.GetUsername(),
                notification.Code);

            var stock = new StockBatch
            {
                WarehouseCode = notification.Data.WareHouseDestinationCode,
                ItemCode = item.ItemCode,
                Cogsidr = item.Price,
                ActualStock = item.Quantity,
                UsedStock = 0,
                CreatedBy = currentUser.GetUsername(),
                CreatedDate = DateTime.Now,
                UpdatedBy = currentUser.GetUsername(),
                UpdatedDate = DateTime.Now
            };

            stock.RecordTransaction(item.Quantity, 0, notification.Code, "");
            await dbContext.StockBatches.AddAsync(stock, cancellationToken);
        }
    }
}
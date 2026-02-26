using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Events;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.EventHandlers.InventoryIn;

public class AddStockOnCreated(PrintingDbContext printingDb, CurrentUserProvider currentUser) : INotificationHandler<InventoryInCreated>
{
    public async Task Handle(InventoryInCreated notification, CancellationToken cancellationToken)
    {
        foreach (var detail in notification.Data.F304s)
        {
            var stock = new StockBatch
            {
                WarehouseCode = notification.Data.WareHouseCode,
                ItemCode = detail.ItemCode,
                Cogsidr = detail.Price,
                ActualStock = detail.Quantity,
                UsedStock = 0,
                CreatedBy = currentUser.GetUsername(),
                CreatedDate = DateTime.Now,
                UpdatedBy = currentUser.GetUsername(),
                UpdatedDate = DateTime.Now
            };
                
            stock.RecordTransaction(detail.Quantity, 0, notification.Code, "");
            await printingDb.StockBatches.AddAsync(stock, cancellationToken);
        }
    }
}
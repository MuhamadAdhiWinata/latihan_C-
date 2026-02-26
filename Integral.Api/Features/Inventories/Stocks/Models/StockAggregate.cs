using SharedKernel.Abstraction;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.Stocks.Models;

public class StockAggregate(StockBatch[] stockBatches)
{
    public void Consume(string itemCode, string warehouseCode, decimal quantity, string author, string refCode)
    {
        var batches = stockBatches
            .Where(x => x.ItemCode == itemCode)
            .Where(x => x.WarehouseCode == warehouseCode)
            .ToList();

        var remaining = quantity;
        foreach (var stock in batches)
        {
            var available = stock.ActualStock - stock.UsedStock;
            if (available <= 0) continue;

            var reduceQty = Math.Min(remaining, available);

            stock.Consume(reduceQty, refCode, "", author);
            remaining -= reduceQty;

            if (remaining <= 0)
                break;
        }

        if (remaining > 0)
            throw new DomainRuleException($"Stock {itemCode} in warehouse {warehouseCode} insufficient");
    }

    public void Reverse(StockTransaction transaction, string author)
    {
        var batch = stockBatches.FirstOrDefault(x => x.Id == transaction.StockId);
        if (batch == null)
            throw new AppException("");

        batch.Reverse(transaction, author);
    }
}
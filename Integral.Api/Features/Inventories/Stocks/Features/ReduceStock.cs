using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.Stocks.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.Stocks.Features;


public record ReduceStock(string ItemCode, decimal Quantity, string? RefCode = null) : ICommand<ReduceStockResult>;
public record ReducedStockDetail(int StockId, string ItemCode, decimal Quantity, decimal Price);
public record ReduceStockResult(ReducedStockDetail[] Data);

public class ReduceStockCommandHandler(PrintingDbContext printingDb, CurrentUserProvider currentUser) : ICommandHandler<ReduceStock, ReduceStockResult>
{
    public async Task<ReduceStockResult> Handle(ReduceStock request, CancellationToken cancellationToken)
    {
        var result = new List<ReducedStockDetail>();

        
        // Load available stock lots (FIFO = oldest first)
        var stocks = await printingDb.StockBatches
            .Where(s => s.ItemCode == request.ItemCode)
            .Where(s => s.ActualStock > s.UsedStock)
            .OrderBy(s => s.Id) // oldest lot first
            .ToListAsync(cancellationToken);

        var remaining = request.Quantity;

        foreach (var stock in stocks)
        {
            var available = stock.ActualStock - stock.UsedStock;
            if (available <= 0) continue;

            // Take as much as needed, but not more than available
            var reduceQty = Math.Min(remaining, available);

            stock.UsedStock += reduceQty;
            stock.UpdatedBy = currentUser.GetUsername();
            stock.UpdatedDate = DateTime.Now;

            await printingDb.StockTransactions.AddAsync(new StockTransaction
            {
                StockId = stock.Id,
                RefCode = request.RefCode,
                Out = reduceQty,
                CreatedBy = currentUser.GetUsername(),
                CreatedDate = DateTime.Now,
                UpdatedBy = "",
                UpdatedDate = DateTime.Now
            }, cancellationToken);
            
            result.Add(new ReducedStockDetail(stock.Id, request.ItemCode, reduceQty, stock.Cogsidr));

            remaining -= reduceQty;

            if (remaining <= 0)
                break; // done
        }

        if (remaining > 0)
            throw new InvalidOperationException(
                $"Not enough stock for item {request.ItemCode}. Short by {remaining}."
            );

        return new ReduceStockResult(result.ToArray());
    }
}
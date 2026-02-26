using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.Stocks.Models;
using MediatR;
using SharedKernel.Abstraction.CQRS;

namespace Integral.Api.Features.Inventories.Stocks.Features;

public record AddStock(string WarehouseCode, string ItemCode, decimal Quantity, decimal Price, string? RefCode = null) : ICommand;

// Handler
public class AddStockHandler(PrintingDbContext printingDb) : ICommandHandler<AddStock>
{
    public async Task<Unit> Handle(AddStock request, CancellationToken cancellationToken)
    {
        // Create a new stock lot entry
        var stock = new StockBatch
        {
            WarehouseCode = request.WarehouseCode,
            ItemCode = request.ItemCode,
            ActualStock = request.Quantity,
            UsedStock = 0,
            Cogsidr = request.Price,
            CreatedBy = "SYSTEM",
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now
        };

        await printingDb.StockBatches.AddAsync(stock, cancellationToken);
        
        printingDb.StockTransactions.Add(new StockTransaction
        {
            StockBatch = stock,
            RefCode = request.RefCode,
            In = request.Quantity,
            CreatedBy = "SYSTEM",
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now
        });

        await printingDb.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
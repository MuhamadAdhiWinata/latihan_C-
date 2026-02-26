using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.Stocks.Models;

public class StockReservation
{
    public long Id { get; set; }

    public string ItemCode { get; set; }

    public string WarehouseCode { get; set; }

    public decimal Quantity { get; set; }

    public string RefNo { get; set; }

    public string Note { get; set; }
    public bool IsApplied { get; set; }

    public string? CreatedBy { get; set; } = "systen";

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string? UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public void Apply(StockBatch[] batches, string note, string author)
    {
        if (IsApplied) throw new InvalidOperationException("Reservation already applied.");
        IsApplied = true;

        var remaining = Quantity;
        foreach (var batch in batches)
        {
            if (batch.ItemCode != ItemCode) continue;
            if (batch.WarehouseCode != WarehouseCode) continue;
            if (batch.Available <= 0) continue;

            if (remaining <= 0) break;

            var available = batch.Available;
            if (available <= 0) continue;

            var consume = Math.Min(available, remaining);
            batch.Consume(consume, RefNo, note, author);
            remaining -= consume;
        }

        if (remaining > 0)
            throw new InsufficientStockRuleException();
    }
}

public class InsufficientStockRuleException : DomainRuleException
{
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.Stocks.Models;

[Table("f399")]
public class StockBatch
{
    [Key] [Column("Code")] public int Id { get; set; }

    public string? BranchCode { get; set; }

    public string WarehouseCode { get; set; } = null!;

    public string ItemCode { get; set; } = null!;

    [Precision(0)] public DateTime ItemDate { get; set; }

    [Column("COGSIDR")] public decimal Cogsidr { get; set; }

    public decimal ActualStock { get; set; }

    public decimal UsedStock { get; set; }

    public long? RefBarangDetails { get; set; }

    public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")] public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public virtual Item Item { get; set; }

    public virtual ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();

    public decimal Available => ActualStock - UsedStock;

    public void Consume(decimal amount, string refNo, string note, string author)
    {
        if (amount > Available)
            throw new DomainRuleException(
                $"Insufficient stock in batch {Id}. Available: {Available}, Requested: {amount}");

        UsedStock += amount;
        UpdatedBy = author;
        UpdatedDate = DateTime.Now;

        RecordTransaction(0, amount, refNo, note);
    }

    public void Reverse(StockTransaction transaction, string author)
    {
        if (transaction.Out > 0) UsedStock -= transaction.Out;

        UpdatedBy = author;
        UpdatedDate = DateTime.Now;

        RecordTransaction(transaction.Out, transaction.In, transaction.RefCode, "", transaction.Id);
    }

    public void RecordTransaction(decimal inbound, decimal outbound, string? refNo, string note,
        long? reversalOf = null)
    {
        Transactions.Add(new StockTransaction
        {
            In = inbound,
            Out = outbound,
            RefCode = refNo,
            ReverseOfId = reversalOf
        });
    }
}
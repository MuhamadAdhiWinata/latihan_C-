using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.Stocks.Models;

public sealed class StockTransaction
{
    [Key] public long Id { get; set; }

    public int StockId { get; set; }
    
    public long? ReverseOfId { get; set; }

    public string? RefCode { get; set; }

    public decimal In { get; set; }

    public decimal Out { get; set; }

    public string? CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [ForeignKey(nameof(StockId))] public StockBatch StockBatch { get; set; } = null!;
    
    
    [ForeignKey(nameof(ReverseOfId))]  
    public StockTransaction? ReversalOf { get; set; } = null;
    public StockTransaction? ReversedBy { get; set; } = null;
}
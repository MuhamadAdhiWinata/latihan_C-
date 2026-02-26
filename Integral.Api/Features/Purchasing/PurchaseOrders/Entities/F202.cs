using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseOrders.Entities;

[Keyless]
[Table("f202")]
public class F202
{
    public string BranchCode { get; set; } = null!;

    [Column("PODNo")] public string Podno { get; set; } = null!;

    public string ItemCode { get; set; } = null!;

    public string UnitCode { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPercent1 { get; set; }

    public decimal DiscountAmount1 { get; set; }

    public decimal DiscountPercent2 { get; set; }

    public decimal DiscountAmount2 { get; set; }

    public decimal DiscountPercent3 { get; set; }

    public decimal DiscountAmount3 { get; set; }

    public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")] public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }
}
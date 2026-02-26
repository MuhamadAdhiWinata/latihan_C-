using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseOrders.Entities;

[Keyless]
[Table("f202b")]
public class F202b
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("POCNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Pocno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string UnitCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent1 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount1 { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent2 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount2 { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent3 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount3 { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Entities;

[PrimaryKey(nameof(Dodno), nameof(ItemCode))]
[Table("f606_b")]
public class WorkOrderItem
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("DODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Dodno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string ItemAlias { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent1 { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent2 { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent3 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount1 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount2 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount3 { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal BonusQuantity { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    
    [InverseProperty(nameof(WorkOrder.Items))]
    [ForeignKey(nameof(Dodno))]
    public virtual WorkOrder WorkOrder { get; set; } = null!;
}
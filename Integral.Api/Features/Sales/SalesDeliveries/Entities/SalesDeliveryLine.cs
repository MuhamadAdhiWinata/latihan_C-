using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Sales.SalesDeliveries.Entities;

[Table("f606")]
public class SalesDeliveryLine
{
    [Key] public long Id { get; set; }
    
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


    // [ForeignKey("BranchCode")]
    // [InverseProperty("F606s")]
    // public virtual F001 BranchCodeNavigation { get; set; } = null!;

    [ForeignKey("Dodno")]
    [InverseProperty(nameof(SalesDelivery.F606s))]
    public virtual SalesDelivery DodnoNavigation { get; set; } = null!;
    //
    // [ForeignKey("ItemCode")]
    // [InverseProperty("F606s")]
    // public virtual Item ItemCodeNavigation { get; set; } = null!;
}
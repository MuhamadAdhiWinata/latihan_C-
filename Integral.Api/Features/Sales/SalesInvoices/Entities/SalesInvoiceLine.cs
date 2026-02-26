using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Sales.SalesInvoices.Entities;

[Table("f608")]
public class SalesInvoiceLine
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("INVNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Invno { get; set; } = null!;

    [Column("BIllNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string BillNo { get; set; } = null!;

    [Precision(0)] public DateTime BillDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string ItemAlias { get; set; } = null!;

    [Column("fbill_typeCode")]
    [StringLength(50)]
    [Unicode(false)]
    public string FbillTypeCode { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column("fstart_mtr", TypeName = "decimal(20, 0)")]
    public decimal FstartMtr { get; set; }

    [Column("ffinis_mtr", TypeName = "decimal(20, 0)")]
    public decimal FfinisMtr { get; set; }

    [Column("fusage", TypeName = "decimal(20, 0)")]
    public decimal Fusage { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal BonusQuantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [Column("PPn", TypeName = "decimal(18, 4)")]
    public decimal Ppn { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal BiayaBeban { get; set; }

    [Column("fbill_pph", TypeName = "decimal(18, 4)")]
    public decimal FbillPph { get; set; }

    [Column("fbill_adm", TypeName = "decimal(18, 4)")]
    public decimal FbillAdm { get; set; }

    [Column("fbill_ppj", TypeName = "decimal(18, 4)")]
    public decimal FbillPpj { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Total { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent1 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount1 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal FinalDiscount1 { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent2 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount2 { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent3 { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount3 { get; set; }

    [Column("fstatus")]
    [StringLength(1)]
    [Unicode(false)]
    public string Fstatus { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    [Key] public long Id { get; set; }

    [ForeignKey("Invno")]
    [InverseProperty("F608s")]
    public virtual SalesInvoice InvnoNavigation { get; set; } = null!;

    public decimal Sum() => Math.Round(Price * Quantity, 2);
}
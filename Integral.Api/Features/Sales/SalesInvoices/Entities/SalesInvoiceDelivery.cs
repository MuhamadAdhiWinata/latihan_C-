using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Sales.SalesInvoices.Entities;

[PrimaryKey("BranchCode", "Invno", "Dodno")]
[Table("f928")]
public class SalesInvoiceDelivery
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string BranchCode { get; set; } = null!;

    [Key]
    [Column("INVNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Invno { get; set; } = null!;

    [Key]
    [Column("DODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Dodno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string? CreatedBy { get; set; }

    [Precision(0)] public DateTime? CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? UpdatedBy { get; set; }

    [Precision(0)] public DateTime? UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    // [ForeignKey("BranchCode")]
    // [InverseProperty("F928s")]
    // public virtual F001 BranchCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("Dodno")]
    // [InverseProperty("F928s")]
    // public virtual F605 DodnoNavigation { get; set; } = null!;

    [ForeignKey("Invno")]
    [InverseProperty("F928s")]
    public virtual SalesInvoice InvnoNavigation { get; set; } = null!;
}
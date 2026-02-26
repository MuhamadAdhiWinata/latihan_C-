using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Finance.AccountPayableGrns.Entities;

[Table("f505_ap")]
public class AccountPayableGrnLine
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("BKKNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Bkkno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string SupplierCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CashTypeCode { get; set; } = null!;

    [Column("BKKTo")]
    [StringLength(20)]
    [Unicode(false)]
    public string Bkkto { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [Unicode(false)] public string Description { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    public short DeleteStatus { get; set; }

    [StringLength(200)] [Unicode(false)] public string Terbilang { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public bool? Approved { get; set; }
    //
    // [ForeignKey("BranchCode")]
    // [InverseProperty("F505Aps")]
    // public virtual F001 BranchCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("SupplierCode")]
    // [InverseProperty("F505Aps")]
    // public virtual F109 SupplierCodeNavigation { get; set; } = null!;
}
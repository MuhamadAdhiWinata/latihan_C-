using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Finance.Cashbook.Entities;

[Keyless]
[Table("f520")]
public class F520
{
    [Column("GJMNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Gjmno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string? DocumentNo { get; set; }

    [StringLength(50)] [Unicode(false)] public string BudgetTypeCode { get; set; } = null!;

    [Column("GJMDescription")]
    [StringLength(100)]
    [Unicode(false)]
    public string Gjmdescription { get; set; } = null!;

    [StringLength(3)] [Unicode(false)] public string TransactionType { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Amount { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    [ForeignKey("Gjmno, BranchCode")] public virtual F519 F519 { get; set; } = null!;
}
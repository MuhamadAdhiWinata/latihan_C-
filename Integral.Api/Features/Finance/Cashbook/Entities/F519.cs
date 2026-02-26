using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Finance.Cashbook.Entities;

[PrimaryKey("Gjmno", "BranchCode")]
[Table("f519")]
public class F519
{
    [Key]
    [Column("GJMNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Gjmno { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string BranchCode { get; set; } = null!;

    [Column("COA")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Coa { get; set; }

    [Column("COAA")]
    [StringLength(50)]
    [Unicode(false)]
    public string Coaa { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    [Unicode(false)] public string Description { get; set; } = null!;

    [Column("CBType")]
    [StringLength(50)]
    [Unicode(false)]
    public string Cbtype { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    public bool Approved { get; set; }
}
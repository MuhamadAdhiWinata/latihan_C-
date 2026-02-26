using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Finance.AccountReceivables.Entities;

[Table("f501")]
public class F501
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("BKMNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Bkmno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CashTypeCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CustomerCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string BlockUnit { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string TradeName { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [Column("BKMFrom")]
    [StringLength(50)]
    [Unicode(false)]
    public string Bkmfrom { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    [Unicode(false)] public string Description { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string Terbilang { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string FakNo { get; set; } = null!;

    [Column("BPno")]
    [StringLength(50)]
    [Unicode(false)]
    public string Bpno { get; set; } = null!;

    public short DeleteStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public bool? Approve { get; set; }
}
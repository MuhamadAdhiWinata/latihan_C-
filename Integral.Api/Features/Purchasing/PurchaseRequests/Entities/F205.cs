using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseRequests.Entities;

[Table("f205")]
public class F205
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("PPno")]
    [StringLength(50)]
    [Unicode(false)]
    public string Ppno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [Column("PPName")]
    [StringLength(50)]
    [Unicode(false)]
    public string Ppname { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string Divisi { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string Jabatan { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [Unicode(false)] public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal GrandTotalAmount { get; set; }

    public short PostLog { get; set; }

    [Precision(0)] public DateTime PostLogDate { get; set; }

    [Column("PostPO")] public short PostPo { get; set; }

    [Column("PostPODate")] [Precision(0)] public DateTime PostPodate { get; set; }

    [Column("PODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Podno { get; set; } = null!;

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

    public short ClosingStatus { get; set; }

    [Precision(0)] public DateTime ClosingDate { get; set; }
}
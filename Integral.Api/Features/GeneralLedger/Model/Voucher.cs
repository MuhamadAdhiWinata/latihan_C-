using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.GeneralLedger.Model;

[PrimaryKey("BranchCode", "VoucherNo")]
[Table("f401")]
public class Voucher
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string BranchCode { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string VoucherNo { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [StringLength(3)] [Unicode(false)] public string JournalType { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string JournalVoucherCode { get; set; } = null!;

    public short PostingStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string ParentVoucherNo { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string AutomaticJournalType { get; set; } = null!;

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
    
    public virtual ICollection<VoucherLine> Lines { get; set; } = new List<VoucherLine>();

    public static Voucher Create(string code, string type, string refNo, string user, VoucherLine[] lines)
    {
        return new Voucher()
        {
            BranchCode = AppDefaults.BranchCode,
            VoucherNo = code,
            TransactionDate = DateTime.UtcNow,
            RefNo = refNo,
            ExchangeRate = AppDefaults.ExchangeRate,
            CurrencyCode = AppDefaults.CurrencyCode,
            JournalType = type,
            JournalVoucherCode = type,
            AutomaticJournalType = type,
            ParentVoucherNo = "",
            CreatedBy = user,
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now,
            
            Lines = lines
        };
    }
}
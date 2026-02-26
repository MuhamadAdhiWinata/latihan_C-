using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.GeneralLedger.Model;

[Table("f402")]
public class VoucherLine
{
    [Key] public long Id { get; set; }
    
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string VoucherNo { get; set; } = null!;

    [Unicode(false)] public string? Description { get; set; }

    [StringLength(22)] [Unicode(false)] public string? AccountCode { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Debit { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Credit { get; set; }

    [StringLength(50)] [Unicode(false)] public string SubAreaCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string? CreatedBy { get; set; }

    [Precision(0)] public DateTime? CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? UpdatedBy { get; set; }

    [Precision(0)] public DateTime? UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }
    
    public virtual Voucher? Header { get; set; }

    public static VoucherLine Create(string voucherCode, string accountCode, decimal debit, decimal credit, string description, string user )
    {
        return new VoucherLine()
        {
            BranchCode = AppDefaults.BranchCode,
            VoucherNo = voucherCode,
            AccountCode = accountCode,
            Debit = debit,
            Credit = credit,
            Description = description,
            SubAreaCode = "",
            CreatedBy = user,
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now,
        };
    }
}
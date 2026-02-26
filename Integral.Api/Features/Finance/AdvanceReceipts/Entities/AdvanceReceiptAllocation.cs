using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Finance.AdvanceReceipts.Entities;

[PrimaryKey("BranchCode", "Adrno")]
[Table("f543")]
public class AdvanceReceiptAllocation
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string BranchCode { get; set; } = null!;

    [Key]
    [Column("ADRNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Adrno { get; set; } = null!;

    public DateTime TransDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CustomerCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string BlockUnit { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string AllocateTo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")] public decimal Allocation { get; set; }

    [Column("COACredit")]
    [StringLength(50)]
    [Unicode(false)]
    public string Coacredit { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column("COADebit")]
    [StringLength(50)]
    [Unicode(false)]
    public string Coadebit { get; set; } = null!;

    public bool Approved { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }
}
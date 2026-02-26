using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Finance.AccountPayables.Entities;

[Table("f506")]
public class AccountPayableLine
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("BKKNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Bkkno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string BudgetTypeCode { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string DocumentNo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Amount { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [StringLength(3)] [Unicode(false)] public string DocumentType { get; set; } = null!;

    [Column("DPP_PPN_Status")]
    [StringLength(3)]
    [Unicode(false)]
    public string DppPpnStatus { get; set; } = null!;

    [Key] public long Id { get; set; }

    [ForeignKey("Bkkno")]
    [InverseProperty("F506s")]
    public virtual AccountPayable BkknoNavigation { get; set; } = null!;
}
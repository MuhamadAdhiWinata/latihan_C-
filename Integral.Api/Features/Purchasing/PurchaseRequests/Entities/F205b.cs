using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseRequests.Entities;

[Table("f205b")]
public class F205b
{
    [StringLength(50)] [Unicode(false)] public string? BranchCode { get; set; }

    [Key]
    [Column("PRCno")]
    [StringLength(50)]
    [Unicode(false)]
    public string Prcno { get; set; } = null!;

    [Column("PPno")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Ppno { get; set; }

    public DateTime? ClosingDate { get; set; }

    [StringLength(150)] [Unicode(false)] public string? Reason { get; set; }

    [Column(TypeName = "text")] public string? Description { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public bool? Approved { get; set; }
}
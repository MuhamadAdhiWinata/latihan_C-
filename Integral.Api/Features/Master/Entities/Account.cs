using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Entities;

[Table("f101")]
public class Account
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string Name { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string Level { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string Type { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string TypeCoa { get; set; } = null!;
}
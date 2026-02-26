using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Entities;

[Table("f015_b")]
public class AccountWarehouse
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)] [Unicode(false)] public string Code { get; set; } = null!;

    [Column("WHCode")]
    [StringLength(100)]
    [Unicode(false)]
    public string Whcode { get; set; } = null!;

    public short ActiveStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string AccountCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string? CreatedBy { get; set; }

    [Precision(0)] public DateTime? CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? UpdatedBy { get; set; }

    [Precision(0)] public DateTime? UpdatedDate { get; set; }
}
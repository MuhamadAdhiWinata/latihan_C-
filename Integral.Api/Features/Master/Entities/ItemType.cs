using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Entities;

[Table("f015")]
public class ItemType
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string? Group { get; set; }

    [StringLength(100)] [Unicode(false)] public string Name { get; set; } = null!;

    public short ActiveStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string AccountCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string AccountCode2 { get; set; } = null!;

    [InverseProperty(nameof(Item.ItemType))]
    public virtual ICollection<Item> Items { get; set; } = null!;
}
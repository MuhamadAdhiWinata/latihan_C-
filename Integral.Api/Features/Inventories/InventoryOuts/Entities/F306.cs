using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.InventoryOuts.Entities;

[Table("f306")]
public class InventoryOutLine
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("IOTNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Iotno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [StringLength(50)] [Unicode(false)] public string ReasonCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    [Key] public long Id { get; set; }

    [ForeignKey("Iotno")]
    [InverseProperty("F306s")]
    public virtual InventoryOut IotnoNavigation { get; set; } = null!;

    [ForeignKey(nameof(ItemCode))]
    public virtual Item ItemNavigation { get; set; } = null!;
}
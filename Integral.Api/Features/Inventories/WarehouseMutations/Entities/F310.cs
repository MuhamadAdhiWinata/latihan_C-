using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.WarehouseMutations.Entities;

[Table("f310")]
public class F310
{
    [Key] public long Id { get; set; }

    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("WHMNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Whmno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Total { get; set; }

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

    [InverseProperty(nameof(F309.Items))]
    [ForeignKey(nameof(Whmno))]
    public virtual F309 WarehouseMutationNavigation { get; set; } = null!;

    // [ForeignKey("BranchCode")] public virtual F001 BranchCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("ItemCode")] public virtual Item ItemCodeNavigation { get; set; } = null!;
}
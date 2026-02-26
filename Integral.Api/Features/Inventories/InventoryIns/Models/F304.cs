using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.InventoryIns.Models;

[Table("f304")]
public class F304
{
    [Key] public long Id { get; set; }
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("IINNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Iinno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [StringLength(50)] [Unicode(false)] public string? ReasonCode { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }


    [InverseProperty(nameof(F303.F304s))]
    [ForeignKey(nameof(Iinno))]
    public virtual F303 F303 { get; set; } = null!;

    [ForeignKey(nameof(ItemCode))] public virtual Item ItemCodeNavigation { get; set; } = null!;
}
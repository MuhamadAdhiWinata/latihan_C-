using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Entities;

[PrimaryKey(nameof(Spk), nameof(ItemCode))]
[Table("F606_b_f")]
public class WorkOrderPackaging
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("SPK")]
    [StringLength(50)]
    [Unicode(false)]
    public string Spk { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string ItemAlias { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }
    
    [InverseProperty(nameof(WorkOrder.Packagings))]
    [ForeignKey(nameof(Spk))]
    public virtual WorkOrder WorkOrder { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseOrders.Entities;

[Table("f201b")]
public class F201b
{
    [StringLength(50)] [Unicode(false)] public string? BranchCode { get; set; }

    [Key]
    [Column("POCNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Pocno { get; set; } = null!;

    [Column("PODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Podno { get; set; }

    public DateTime? ClosingDate { get; set; }

    [StringLength(100)] [Unicode(false)] public string? SupplierCode { get; set; }

    [StringLength(150)] [Unicode(false)] public string? Reason { get; set; }

    [Column(TypeName = "text")] public string? Description { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public bool? Approved { get; set; }
}
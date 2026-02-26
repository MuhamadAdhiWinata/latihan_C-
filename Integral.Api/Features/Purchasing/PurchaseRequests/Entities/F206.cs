using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseRequests.Entities;

[Keyless]
[Table("f206")]
public class F206
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Column("PPno")]
    [StringLength(50)]
    [Unicode(false)]
    public string Ppno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string UnitCode { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string SpecificDes { get; set; } = null!;

    [Column("PPQuantity", TypeName = "decimal(18, 4)")]
    public decimal Ppquantity { get; set; }

    public DateOnly DateNeed { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column("AccPOQuantity", TypeName = "decimal(18, 4)")]
    public decimal AccPoquantity { get; set; }

    [Column("SupplierCode_v1")]
    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCodeV1 { get; set; } = null!;

    [Column("Price_v1", TypeName = "decimal(18, 4)")]
    public decimal PriceV1 { get; set; }

    [Column("DateArived_v1")] public DateOnly DateArivedV1 { get; set; }

    [Column("PostAcc_v1")] public short PostAccV1 { get; set; }

    [Column("SupplierCode_v2")]
    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCodeV2 { get; set; } = null!;

    [Column("Price_v2", TypeName = "decimal(18, 4)")]
    public decimal PriceV2 { get; set; }

    [Column("DateArived_v2")] public DateOnly DateArivedV2 { get; set; }

    [Column("PostAcc_v2")] public short PostAccV2 { get; set; }

    [Column("SupplierCode_v3")]
    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCodeV3 { get; set; } = null!;

    [Column("Price_v3", TypeName = "decimal(18, 4)")]
    public decimal PriceV3 { get; set; }

    [Column("DateArived_v3")] public DateOnly DateArivedV3 { get; set; }

    [Column("PostAcc_v3")] public short PostAccV3 { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }
}
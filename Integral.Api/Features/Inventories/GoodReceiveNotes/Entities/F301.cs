using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.GoodReceiveNotes.Entities;

[Table("f301")]
public class F301
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("GRNNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Grnno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [Column("PODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Podno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string SupplierCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [StringLength(50)] [Unicode(false)] public string PaymentTermCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string WarehouseCode { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string ReceivedBy { get; set; } = null!;

    [Unicode(false)] public string Description { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string SupplierRefNo { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string PoliceNo { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ContainerNo { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount { get; set; }

    [Column("PPNPercent", TypeName = "decimal(18, 4)")]
    public decimal Ppnpercent { get; set; }

    [Column("PPNAmount", TypeName = "decimal(18, 4)")]
    public decimal Ppnamount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal FreightFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal InsuranceFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal AdministrationFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal OtherFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal RoundedAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal GrandTotal { get; set; }

    public short DeleteStatus { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal PaidAmount { get; set; }

    [StringLength(50)] [Unicode(false)] public string SettlementDocumentNo { get; set; } = null!;

    [Precision(0)] public DateTime SettlementDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("PostGRN")] public bool PostGrn { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    // [ForeignKey("BranchCode")]
    // [InverseProperty("F301s")]
    // public virtual F001 BranchCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("CurrencyCode")]
    // [InverseProperty("F301s")]
    // public virtual F006 CurrencyCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("PaymentTermCode")]
    // [InverseProperty("F301s")]
    // public virtual F018 PaymentTermCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("SupplierCode")]
    // [InverseProperty("F301s")]
    // public virtual F109 SupplierCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("WarehouseCode")]
    // [InverseProperty("F301s")]
    // public virtual F110 WarehouseCodeNavigation { get; set; } = null!;
}
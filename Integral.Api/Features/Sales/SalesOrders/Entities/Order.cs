using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Sales.SalesOrders.Entities;

[Table("f603")]
public class Order
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("SODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Sodno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [Precision(0)] public DateTime DeliveryDateStart { get; set; }

    [Precision(0)] public DateTime DeliveryDateEnd { get; set; }

    [Precision(0)] public DateTime ExpiredDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string PaymentTermCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CustomerCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string SalesmanCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [StringLength(255)] [Unicode(false)] public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal RiskPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal RiskAmount { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal AddPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal AddAmount { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal MarginPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal MarginAmount { get; set; }

    [Column("PPNPercent", TypeName = "decimal(4, 2)")]
    public decimal Ppnpercent { get; set; }

    [Column("PPNAmount", TypeName = "decimal(18, 4)")]
    public decimal Ppnamount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal FreightFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal InsuranceFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal AdministrationFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal OtherFeeAmount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal FinalDiscount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal GrandTotalAmount { get; set; }

    public bool DeleteStatus { get; set; }

    [Column("CustomerPODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string CustomerPodno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CustomerContact { get; set; } = null!;

    public bool ClosingStatus { get; set; }

    [Precision(0)] public DateTime ClosingDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    public bool TaxStatus { get; set; }

    [Column("isprepress")] public bool Isprepress { get; set; }

    [Column("ispress")] public bool Ispress { get; set; }

    [Column("isfinishing")] public bool Isfinishing { get; set; }

    [Column("ispackaging")] public bool Ispackaging { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public bool? Approved { get; set; }

    public bool? Approved2 { get; set; }

    [Column("DPPAmount", TypeName = "decimal(38, 0)")]
    public decimal Dppamount { get; set; }

    [Column("PPNEffectivePercent", TypeName = "decimal(38, 0)")]
    public decimal PpneffectivePercent { get; set; }

    [InverseProperty(nameof(OrderItem.SodnoNavigation))] 
    public virtual ICollection<OrderItem> F604s { get; set; } = new List<OrderItem>();

    public decimal DppPercent => PpneffectivePercent / Ppnpercent;
}
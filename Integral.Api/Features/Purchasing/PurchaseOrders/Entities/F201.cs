using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Purchasing.PurchaseOrders.Entities;

[Table("f201")]
public class F201
{
    public string BranchCode { get; set; } = null!;

    [Key]
    [Column("PODNo")]
    public string Podno { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public DateTime DeliveryDateStart { get; set; }

    public DateTime DeliveryDateEnd { get; set; }

    public DateTime ExpiredDate { get; set; }

    public string PaymentTermCode { get; set; } = null!;

    public string SupplierCode { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public string RefNo { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal TotalTransactionAmount { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    [Column("PPNPercent")]
    public decimal Ppnpercent { get; set; }

    [Column("PPNAmount")]
    public decimal Ppnamount { get; set; }

    public decimal FreightFeeAmount { get; set; }

    public decimal InsuranceFeeAmount { get; set; }

    public decimal AdministrationFeeAmount { get; set; }

    public decimal OtherFeeAmount { get; set; }

    public decimal GrandTotalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public string GoodsDestinationCode { get; set; } = null!;

    public string InvoiceDestinationCode { get; set; } = null!;

    public short ClosingStatus { get; set; }

    public DateTime ClosingDate { get; set; }

    [Column("PODStatus")]
    [StringLength(10)]
    [Unicode(false)]
    public string Podstatus { get; set; } = null!;

    public string ApprovedBy { get; set; } = null!;

    public short DeleteStatus { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public string ApproveJobPosition { get; set; } = null!;

    public string SupplierApprovedBy { get; set; } = null!;

    public string SupplierApprovedJobPosition { get; set; } = null!;

    public decimal RoundedAmount { get; set; }

    [StringLength(1)] public string StatusLampiran { get; set; } = null!;

    [StringLength(200)] public string RekapDes { get; set; } = null!;

    public short PostStatus { get; set; }

    public short PostStatus2 { get; set; }

    [Column("ALTERdBy")]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public string? ShipmentType { get; set; }
}
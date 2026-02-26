using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Sales.SalesDeliveries.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Sales.SalesInvoices.Entities;

[Table("f607")]
public class SalesInvoice : Aggregate
{
    [Key]
    [Column("INVNo")]
    public string Invno { get; set; } = null!;
    public string BranchCode { get; set; } = null!;

    public short PostingSend { get; set; }

    public short PostingRec { get; set; }


    public string FakNo { get; set; } = null!;

    public DateTime FakDate { get; set; }

    [Column("BPno")]
    public string Bpno { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public short StatusFak { get; set; }

    [StringLength(1)] [Unicode(false)] public string StatusRef { get; set; } = null!;

    public string FakNoRef { get; set; } = null!;

    public DateTime FakRefDate { get; set; }

    [Column("SODNo")]
    public string Sodno { get; set; } = null!;

    public string VoucherNo { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public DateOnly PrintDate { get; set; }

    public string BankAccountCode { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;

    public string PaymentTermCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public string CustomerCode { get; set; } = null!;

    [Column("trade_name")]
    [StringLength(200)]
    [Unicode(false)]
    public string TradeName { get; set; } = null!;

    public string SalesmanCode { get; set; } = null!;

    public string WarehouseCode { get; set; } = null!;

    [Column("dept_code")]
    public string DeptCode { get; set; } = null!;

    [Unicode(false)] public string Description { get; set; } = null!;

    public short TaxStatus { get; set; }

    public decimal TotalTransaction { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalDiscount { get; set; }

    [Column("OtherFeeBeforePPN", TypeName = "decimal(18, 4)")]
    public decimal OtherFeeBeforePpn { get; set; }

    [Column("PPNPercent", TypeName = "decimal(4, 2)")]
    public decimal Ppnpercent { get; set; }

    [Column("fbill_ppj", TypeName = "decimal(18, 4)")]
    public decimal FbillPpj { get; set; }

    [Column("PPNAmount", TypeName = "decimal(18, 4)")]
    public decimal Ppnamount { get; set; }

    [Column("OtherFeeAfterPPN", TypeName = "decimal(18, 4)")]
    public decimal OtherFeeAfterPpn { get; set; }

    [Column("MateraiFeeAfterPPN", TypeName = "decimal(18, 4)")]
    public decimal MateraiFeeAfterPpn { get; set; }

    [Column("PackingFeeAfterPPN", TypeName = "decimal(18, 4)")]
    public decimal PackingFeeAfterPpn { get; set; }

    [Column("AsuransiFeeAfterPPN", TypeName = "decimal(18, 4)")]
    public decimal AsuransiFeeAfterPpn { get; set; }

    [Column("PPh23FeeAfterPPN", TypeName = "decimal(18, 4)")]
    public decimal Pph23FeeAfterPpn { get; set; }

    [Column("DeductionAfterPPn", TypeName = "decimal(18, 4)")]
    public decimal DeductionAfterPpn { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal PaidAmount { get; set; }

    [Column(TypeName = "decimal(1, 0)")] public decimal AssignStatus { get; set; }

    public DateTime AssignDate { get; set; }

    public short DeleteStatus { get; set; }

    public DateTime SettlementDate { get; set; }

    public string SettlementDocumentNo { get; set; } = null!;

    [Unicode(false)] public string PayDescription { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public string RefNo { get; set; } = null!;

    [Column("ALTERdBy")]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    [Column("DeliveryFeeBeforePPN", TypeName = "decimal(18, 4)")]
    public decimal DeliveryFeeBeforePpn { get; set; }

    [Column("DPPAmount", TypeName = "decimal(38, 0)")]
    public decimal Dppamount { get; set; }

    [Column("PPNEffectivePercent", TypeName = "decimal(38, 0)")]
    public decimal PpneffectivePercent { get; set; }

    [InverseProperty("InvnoNavigation")] public virtual ICollection<SalesInvoiceLine> F608s { get; set; } = new List<SalesInvoiceLine>();

    [InverseProperty("InvnoNavigation")] public virtual ICollection<SalesInvoiceDelivery> F928s { get; set; } = new List<SalesInvoiceDelivery>();

    public static SalesInvoice Create(string code, DateTime transactionDate, string branchCode)
    {
        var invoice = new SalesInvoice()
        {
            Invno = code,
            TransactionDate = transactionDate,
            BranchCode = branchCode

        };

        invoice.AddDomainEvent(new SalesInvoiceCreated(invoice.Invno));
        return invoice;
    }

    public void Calculate()
    {
        DiscountPercent = Math.Round(DiscountAmount / TotalTransaction * 100, 2);

        var subtotal = TotalTransaction - DiscountAmount + OtherFeeBeforePpn + DeliveryFeeBeforePpn;

        Dppamount = PpneffectivePercent > 0 ? Math.Round(subtotal * PpneffectivePercent / Ppnpercent, 2) : subtotal;
        Ppnamount = Math.Round(Dppamount * Ppnpercent / 100, 2);
        
        GrandTotal = subtotal
                     + Ppnamount
                     + OtherFeeAfterPpn
                     + MateraiFeeAfterPpn
                     + PackingFeeAfterPpn
                     + AsuransiFeeAfterPpn
                     + Pph23FeeAfterPpn
                     + DeductionAfterPpn;
    }

    public void AddDelivery(SalesDelivery delivery)
    {
        F928s.Add(new SalesInvoiceDelivery()
        {
            BranchCode = BranchCode,
            Invno = Invno,
            Dodno = delivery.Dodno,
        });

        foreach (var deliveryLine in delivery.F606s)
        {
            var inventoryLine = F608s.FirstOrDefault(x => x.ItemCode == deliveryLine.ItemCode && x.Price == deliveryLine.Price);
            if (inventoryLine == null)
            {
                F608s.Add(new SalesInvoiceLine()
                {
                    Invno = Invno,
                    ItemCode = deliveryLine.ItemCode,
                    ItemAlias = deliveryLine.ItemAlias,
                    Price = deliveryLine.Price,
                    Quantity = deliveryLine.Quantity,
                    Total = deliveryLine.Price * deliveryLine.Quantity,
                });
            }
            else
            {
                 inventoryLine.Quantity += deliveryLine.Quantity;
                 inventoryLine.Total =  deliveryLine.Price * deliveryLine.Quantity;
            }
        }
    }

    public void Approve()
    {
        PostingRec = 1;
        PostingSend = 1;
        
        AddDomainEvent(new SalesInvoiceApproved(Invno));
    }
}

public record SalesInvoiceApproved(string Code) : IDomainEvent;

public record SalesInvoiceCreated(string Code) : IDomainEvent;
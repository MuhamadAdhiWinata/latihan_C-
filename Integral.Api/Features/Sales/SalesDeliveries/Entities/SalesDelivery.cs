using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Sales.SalesDeliveries.Entities;

[Table("f605")]
public class SalesDelivery : Aggregate
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("DODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Dodno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CustomerCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string SalesmanCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string DriverCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ExpeditionCode { get; set; } = null!;

    [Column("SODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Sodno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string WarehouseCode { get; set; } = null!;

    public short TaxStatus { get; set; }

    [Unicode(false)] public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransaction { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal DiscountAmount { get; set; }

    [Column("PPNPercent", TypeName = "decimal(4, 2)")]
    public decimal Ppnpercent { get; set; }

    [Column("PPNAmount", TypeName = "decimal(18, 4)")]
    public decimal Ppnamount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal GrandTotal { get; set; }

    public short DeleteStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("SPDescription")]
    [Unicode(false)]
    public string Spdescription { get; set; } = null!;

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public bool? Approved { get; set; }

    public bool? Approved2 { get; set; }

    [InverseProperty(nameof(SalesDeliveryLine.DodnoNavigation))] 
    public virtual ICollection<SalesDeliveryLine> F606s { get; set; } = new List<SalesDeliveryLine>();

    // [InverseProperty("DodnoNavigation")] public virtual ICollection<F928> F928s { get; set; } = new List<F928>();


    public void Approve()
    {
        Approved = true;
        Approved2 = true;
        
        AddDomainEvent(new DeliveryApproved(Dodno));
    }
    
    public static SalesDelivery Create(
        string code, DateTime transactionDate, 
        string customerCode, string orderCode, string warehouseCode, 
        string description, bool taxStatus,
        string author,decimal totalTransaction,
        decimal ppnAmount, decimal ppnPercent,
        SalesDeliveryLine[] deliveryLines)
    {
        var delivery = new SalesDelivery()
        {
            BranchCode = AppDefaults.BranchCode,
            Dodno = code,
            TransactionDate = transactionDate,
            CustomerCode = customerCode,
            Sodno = orderCode,
            WarehouseCode = warehouseCode,
            CurrencyCode = AppDefaults.CurrencyCode,
            ExchangeRate = AppDefaults.ExchangeRate,
            Description = description,
            TaxStatus = (short)(taxStatus ? 1 : 0),
            CreatedBy = author,
            CreatedDate = DateTime.Now,
            DriverCode = "",
            UpdatedBy = "",
            UpdatedDate = DateTime.Now,
            ExpeditionCode = "",
            DiscountAmount =   0,
            SalesmanCode =  "",
            Spdescription = "",
            DiscountPercent = 0,
            DeleteStatus = 0,
            Ppnamount = ppnAmount,
            TotalTransaction = totalTransaction,
            GrandTotal = ppnAmount + totalTransaction,
            Ppnpercent = ppnPercent
            

        };

        foreach (var deliveryLine in deliveryLines)
        {
            delivery.F606s.Add(new SalesDeliveryLine
            {
                BranchCode = AppDefaults.BranchCode,
                UpdatedBy = "",
                ItemCode = deliveryLine.ItemCode,
                ItemAlias = deliveryLine.ItemAlias,
                Quantity = deliveryLine.Quantity,
                Price = deliveryLine.Price,
                CreatedBy = deliveryLine.CreatedBy,
                CreatedDate = DateTime.Now,
                Description = deliveryLine.Description,
                UpdatedDate = DateTime.Now,
                Dodno = deliveryLine.Dodno,
                DiscountAmount1 = 0,
                DiscountAmount2 = 0,
                DiscountAmount3 = 0,
                DiscountPercent = 0,
                DiscountPercent1 = 0,
                DiscountPercent2 = 0,
                DiscountPercent3 = 0,
                BonusQuantity = 0
                
            });
        }
        
        delivery.AddDomainEvent(new DeliveryCreated(delivery.Dodno));
        return delivery;
    }

    public void Delete()
    {
        if (Approved == true || Approved2 == true)
            throw new DomainRuleException("Delivery already approved");
        
        if (DeleteStatus > 0)
            throw new DomainRuleException("Delivery already deleted");

        AddDomainEvent(new DeliveryDeleted(Dodno));
    }
 
    public void Update(DateTime transactionDate, 
        string customerCode, string orderCode, string warehouseCode, 
        string description, bool taxStatus,
        string author,
        SalesDeliveryLine[] deliveryLines)
    {
        if (Approved == true || Approved2 == true)
            throw new DomainRuleException("Delivery already approved");
        
        if (DeleteStatus > 0)
            throw new DomainRuleException("Delivery already deleted");

        BranchCode = AppDefaults.BranchCode;
        TransactionDate = transactionDate;
        CustomerCode = customerCode;
        Sodno = orderCode;
        WarehouseCode = warehouseCode;
        CurrencyCode = AppDefaults.CurrencyCode;
        ExchangeRate = AppDefaults.ExchangeRate;
        Description = description;
        TaxStatus = (short)(taxStatus ? 1 : 0);
        UpdatedBy = author;
        UpdatedDate = DateTime.Now;
        Approved = false;
        Approved2 = false;
        
        F606s.Clear();
        foreach (var deliveryLine in deliveryLines)
        {
            F606s.Add(new SalesDeliveryLine
            {
                BranchCode = AppDefaults.BranchCode,
                UpdatedBy = "",
                ItemCode = deliveryLine.ItemCode,
                ItemAlias = deliveryLine.ItemAlias,
                Quantity = deliveryLine.Quantity,
                Price = deliveryLine.Price,
                CreatedBy = deliveryLine.CreatedBy,
                CreatedDate = DateTime.Now,
                Description = deliveryLine.Description,
                UpdatedDate = DateTime.Now,
                Dodno = deliveryLine.Dodno,
                DiscountAmount1 = 0,
                DiscountAmount2 = 0,
                DiscountAmount3 = 0,
                DiscountPercent = 0,
                DiscountPercent1 = 0,
                DiscountPercent2 = 0,
                DiscountPercent3 = 0,
                BonusQuantity = 0
            }); 
        }

        AddDomainEvent(new DeliveryDeleted(Dodno));
    }
   
}

public record DeliveryApproved(string Code) : IDomainEvent;
public record DeliveryCreated(string Code) : IDomainEvent;
public record DeliveryDeleted(string Code) : IDomainEvent;
public record DeliveryUpdated(string Code) : IDomainEvent;

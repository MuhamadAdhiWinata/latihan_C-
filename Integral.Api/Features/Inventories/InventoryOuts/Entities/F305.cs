using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.InventoryOuts.Entities;

[Table("f305")]
public class InventoryOut : Aggregate
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("IOTNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Iotno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CurrencyCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal ExchangeRate { get; set; }

    [StringLength(50)] [Unicode(false)] public string WareHouseCode { get; set; } = null!;

    [Unicode(false)] public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    public short DeleteStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;


    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    [Column("PostIOT")] public bool PostIot { get; set; }

    [InverseProperty("IotnoNavigation")]
    public virtual ICollection<InventoryOutLine> F306s { get; set; } = new List<InventoryOutLine>();


    public static InventoryOut Create(
        string code, DateTime transactionDate,
        string refNo, string warehouseCode,
        string description, string author,
        InventoryOutLine[] inventoryOutLines)
    {
        var inventoryOut = new InventoryOut()
        {
            BranchCode = AppDefaults.BranchCode,
            Iotno = code,
            TransactionDate = transactionDate,
            WareHouseCode = warehouseCode,
            CurrencyCode = AppDefaults.CurrencyCode,
            ExchangeRate = AppDefaults.ExchangeRate,
            Description = description,
            CreatedBy = author,
            CreatedDate = DateTime.Now,
            RefNo = refNo,
            TotalTransactionAmount = 0,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now,
        };

        foreach (var iout in inventoryOutLines)
        {
            inventoryOut.F306s.Add(new InventoryOutLine()
            {
                ItemCode = iout.ItemCode,
                ReasonCode = iout.ReasonCode,
                Quantity = iout.Quantity,
                Price = 0,
                CreatedBy = iout.CreatedBy,
                CreatedDate = DateTime.Now,
                BranchCode = AppDefaults.BranchCode,
                Description = iout.Description,
                UpdatedBy = "",
                UpdatedDate = DateTime.Now
            });
        }

        inventoryOut.AddDomainEvent(new InventoryOutCreated(inventoryOut.Iotno, inventoryOut));
        return inventoryOut;
    }

    public void Delete()
    {
        AddDomainEvent(new InventoryOutDeleted(Iotno, this));
    }

    public void Update(
        DateTime transactionDate,
        string refNo,
        string warehouseCode,
        string description,
        string author,
        InventoryOutLine[] Items)
    {
        if (DeleteStatus > 0)
            throw new DomainRuleException("Delivery already deleted");
        BranchCode = AppDefaults.BranchCode;
        UpdatedBy = author;
        UpdatedDate = DateTime.Now;
        RefNo = refNo;
        TransactionDate = transactionDate;
        Description = description;
        WareHouseCode = warehouseCode;
        CurrencyCode = AppDefaults.CurrencyCode;
        ExchangeRate = AppDefaults.ExchangeRate;


        F306s.Clear();
        foreach (var ioutLine in Items)
        {
            F306s.Add(new InventoryOutLine()
            {
                Iotno = ioutLine.Iotno,
                ItemCode = ioutLine.ItemCode,
                ReasonCode = ioutLine.ReasonCode,
                Quantity = ioutLine.Quantity,
                Price = 0,
                CreatedBy = ioutLine.CreatedBy,
                CreatedDate = DateTime.Now,
                BranchCode = AppDefaults.BranchCode,
                Description = ioutLine.Description,
                UpdatedBy = author,
                UpdatedDate = DateTime.Now
            });
        }


        AddDomainEvent(new InventoryOutUpdated(Iotno, this));
    }
}

public record InventoryOutCreated(string Code, InventoryOut Data) : IDomainEvent;

public record InventoryOutDeleted(string Code, InventoryOut Data) : IDomainEvent;

public record InventoryOutUpdated(string Code, InventoryOut Data) : IDomainEvent;
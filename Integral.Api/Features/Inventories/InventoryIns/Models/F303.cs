using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Inventories.InventoryIns.Events;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.InventoryIns.Models;

[Table("f303")]
public class F303 : Aggregate
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("IINNo")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(50)]
    [Unicode(false)]
    public string Iinno { get; set; } = null!;

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

    [Column("PostIIN")] public bool PostIin { get; set; }


    [InverseProperty(nameof(F304.F303))] public virtual ICollection<F304> F304s { get; set; } = new List<F304>();

    public void Calculate()
    {
        TotalTransactionAmount = F304s.Sum(x => x.Price * x.Quantity);
    }
    
    public static F303 Create(
        string code, DateTime transactionDate, string branchCode,
        string refNo, string currencyCode, decimal exchangeRate, 
        string warehouseCode, string description, string author,
        IEnumerable<F304> lines)
    {
        var entry =  new F303()
        {
            BranchCode = "INT",
            Iinno = code,
            TransactionDate = transactionDate,
            RefNo = refNo,
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            WareHouseCode = warehouseCode,
            Description = description,
            CreatedBy = author,
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now,
        };

        foreach (var line in  lines)
        {
            entry.F304s.Add(line);
        }
        
        entry.Calculate();
        entry.AddDomainEvent(new InventoryInCreated(entry.Iinno, entry));

        return entry;
    }
}


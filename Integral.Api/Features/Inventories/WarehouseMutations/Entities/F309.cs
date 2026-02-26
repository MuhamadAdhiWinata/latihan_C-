using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.WarehouseMutations.Entities;

[Table("f309")]
public class F309 : Aggregate
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("WHMNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Whmno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal TotalTransactionAmount { get; set; }

    [StringLength(50)] [Unicode(false)] public string WareHouseSourceCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string WareHouseDestinationCode { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string RefNo { get; set; } = null!;

    [Unicode(false)] public string Description { get; set; } = null!;

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

    public bool Approved { get; set; }

    [InverseProperty(nameof(F310.WarehouseMutationNavigation))]
    public virtual ICollection<F310> Items { get; set; } = new HashSet<F310>();

    public static F309 Create(
        string code, DateTime transactionDate, string sourceWarehouseCode,
        string destinationWarehouseCode, string description, string author, IEnumerable<F310> lines, string refNo = "")
    {
        var entry = new F309
        {
            BranchCode = AppDefaults.BranchCode,
            Whmno = code,
            TransactionDate = transactionDate,
            WareHouseSourceCode = sourceWarehouseCode,
            WareHouseDestinationCode = destinationWarehouseCode,
            Description = description,
            RefNo = refNo,

            CreatedBy = author,
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now
        };

        foreach (var line in lines)
        {
            entry.Items.Add(line);
        }

        entry.CalculateTotal();
        entry.AddDomainEvent(new WarehouseMutationCreated(entry.Whmno, entry));

        return entry;
    }

    private void CalculateTotal()
    {
        TotalTransactionAmount = Items.Sum(x => x.Price * x.Quantity);
    }

    // [ForeignKey("BranchCode")]
    // [InverseProperty("F309s")]
    // public virtual F001 BranchCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("WareHouseDestinationCode")]
    // [InverseProperty("F309WareHouseDestinationCodeNavigations")]
    // public virtual F110 WareHouseDestinationCodeNavigation { get; set; } = null!;
    //
    // [ForeignKey("WareHouseSourceCode")]
    // [InverseProperty("F309WareHouseSourceCodeNavigations")]
    // public virtual F110 WareHouseSourceCodeNavigation { get; set; } = null!;
}

public record WarehouseMutationCreated(string Code, F309 Data) : IDomainEvent;
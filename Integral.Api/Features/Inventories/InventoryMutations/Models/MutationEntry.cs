using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Inventories.InventoryMutations.Events;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.InventoryMutations.Models;

public class MutationEntry : Aggregate
{
    [Key] public int Id { get; set; }

    [StringLength(50)] public string TransactionCode { get; set; } = null!;

    [Column(TypeName = "datetime")] public DateTime TransactionDate { get; set; }

    public string? RefNo { get; set; }

    public string Description { get; set; } = "";
    
    public string Status { get; set; } = MutationStatus.PENDING;
    
    public string WarehouseCode { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; } = null!;
    
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; } = null!;

    [InverseProperty("MutationEntry")]
    public virtual ICollection<MutationItem> MutationItems { get; set; } = new List<MutationItem>();

    public void Approve()
    {
        Status = MutationStatus.APPROVED;
        AddDomainEvent(new MutationTransferApproved(TransactionCode,
            MutationItems.Select(x => new MutationTransferApprovedDetail(x.Id, x.ItemCode, x.ItemName, x.Quantity, x.Price, x.SellingPrice)).ToArray()));
    }

    public static MutationEntry Create(string code, DateTime transactionDate, string warehouseCode, string refNo, string description, string author, MutationItem[] mutationItem)
    {
        var entry = new MutationEntry()
        {
            TransactionCode = code,
            TransactionDate = transactionDate,
            RefNo = refNo,
            WarehouseCode = warehouseCode,
            Description = description,
            CreatedBy = author,
            CreatedDate = DateTime.Now,
        };
    
        foreach (var item in mutationItem)
        {
            entry.MutationItems.Add(item);
        }
        
        entry.AddDomainEvent(new MutationCreated(code, entry));
        
        return entry;
    }

    public void Update(DateTime transactionDate, string warehouseCode, string refNo, string description, string author,
        MutationItem[] mutationItem)
    {
        TransactionDate = transactionDate;
        RefNo = refNo;
        Description = description;
        UpdatedBy = author;
        UpdatedDate = DateTime.Now;
        
        MutationItems.Clear();
        foreach (var item in mutationItem)
        {
            MutationItems.Add(item);
        }
        
        AddDomainEvent(new MutationUpdated(TransactionCode, this));
    }

    public void Delete()
    {
        AddDomainEvent(new MutationDeleted(TransactionCode, this));
    }
}

public record MutationCreated(string Code, MutationEntry Data) : IDomainEvent;

public record MutationUpdated(string Code, MutationEntry Data) : IDomainEvent;

public record MutationDeleted(string Code, MutationEntry Data) : IDomainEvent;
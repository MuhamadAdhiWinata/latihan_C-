using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Entities;
using Integral.Api.Features.Master.Items.Events;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Master.Items.Models;

[Table("f013")]                                                                                                                                                                                                                                                                      
public class Item : Aggregate
{
    [Key]
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string UnitCode { get; set; } = null!;

    public decimal MaxStock { get; set; }

    public decimal MinStock { get; set; }

    public string ItemTypeCode { get; set; } = null!;

    public decimal Price { get; set; }

    public short ActiveStatus { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    public string MerkCode { get; set; } = null!;

    public short PackageStatus { get; set; }

    public string? BranchCode { get; set; }

    public string? Sku { get; set; }

    [ForeignKey(nameof(ItemTypeCode))] public virtual ItemType ItemType { get; set; } = null!;

    public static Item Create(string code, string name, string unitCode, decimal price, string sku, string itemTypeCode)
    {
        var item = new Item
        {
            Code = code,
            Name = name,
            UnitCode = unitCode,
            Price = price,
            Sku = sku,
            ItemTypeCode = itemTypeCode,
            ActiveStatus = 1,
            MerkCode = "",
            CreatedBy = "",
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now
        };

        item.AddDomainEvent(new ItemCreated(code));

        return item;
    }
    
    public void Modify(string name, string unitCode, decimal price, string sku, string itemTypeCode)
    {
        Name = name;
        UnitCode = unitCode;
        Price = price;
        Sku = sku;
        ItemTypeCode = itemTypeCode;
        
        AddDomainEvent(new ItemModified(Code));
    }
}
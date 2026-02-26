using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;

namespace Integral.Api.Features.Inventories.InventoryMutations.Models;

[Table("MutationItems")]
public class MutationItem
{
    [Key] public int Id { get; set; }

    [StringLength(50)] public string ItemCode { get; set; } = null!;

    [StringLength(200)] public string ItemName { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")] public decimal Quantity { get; set; }
    [Column(TypeName = "decimal(18, 2)")] public decimal Price { get; set; }
    [Column(TypeName = "decimal(18, 2)")] public decimal SellingPrice { get; set; }
    
    
    public int MutationEntryId { get; set; }

    [ForeignKey(nameof(MutationEntryId))]
    [InverseProperty(nameof(MutationEntry.MutationItems))]
    public virtual MutationEntry MutationEntry { get; set; } = null!;

    [ForeignKey(nameof(ItemCode))] public virtual Item? ItemNavigation { get; set; }
}
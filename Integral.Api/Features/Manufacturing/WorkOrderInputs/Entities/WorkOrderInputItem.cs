using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;

public class WorkOrderInputItem
{
    [Key] public long Id { get; set; }

    public long WorkOrderInputId { get; set; }

    [StringLength(50)] [Unicode(false)] public string ItemCode { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal Price { get; set; }

    [StringLength(200)] [Unicode(false)] public string Description { get; set; } = null!;


    [InverseProperty(nameof(WorkOrderInput.Items))]
    [ForeignKey(nameof(WorkOrderInputId))]
    public virtual WorkOrderInput WorkOrderInput { get; set; } = null!;

    [ForeignKey(nameof(ItemCode))] public virtual Item ItemNavigation { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Events;
using Integral.Api.Features.Manufacturing.WorkOrders.Events;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Ef;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;

public class WorkOrderInput : Aggregate, ICreatedAuditable, IUpdatedAuditable
{
    [Key] public long Id { get; set; }

    [StringLength(50)] [Unicode(false)] public string Code { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string WorkOrderCode { get; set; }

    [Unicode(false)] public string Description { get; set; }

    public decimal TotalTransactionAmount { get; set; }

    public string Status { get; set; } = WorkOrderInputStatus.Pending;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? UpdatedBy { get; set; }

    [Precision(0)] public DateTime? UpdatedDate { get; set; }

    [InverseProperty(nameof(WorkOrderInputItem.WorkOrderInput))]
    public virtual ICollection<WorkOrderInputItem> Items { get; set; } = new List<WorkOrderInputItem>();

    public void Approve()
    {
        if (Items.Count == 0)
            throw new DomainRuleException("Cannot have 0 item requested");
        
        if (Status == WorkOrderInputStatus.Approved)
            throw new DomainRuleException("Already approved");

        Status = WorkOrderInputStatus.Approved;

        AddDomainEvent(new WorkOrderInputApproved(
            Code,
            WorkOrderCode,
            TransactionDate,
            Description,
            Items.Select(x => new WorkOrderInputApprovedItem(x.ItemCode, x.Quantity, x.Price, x.Description))
                .ToArray()));
    }
}
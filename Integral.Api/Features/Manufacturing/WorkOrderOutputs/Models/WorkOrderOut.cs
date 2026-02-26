using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Exceptions;
using Integral.Api.Features.Manufacturing.WorkOrders.Events;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Ef;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Models;

[Index(nameof(Code), nameof(WorkOrderCode))]
public class WorkOrderOut : Aggregate, ICreatedAuditable, IUpdatedAuditable
{
    [Key] public long Id { get; set; }

    [StringLength(50)] [Unicode(false)] public string Code { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string WorkOrderCode { get; set; }

    [Unicode(false)] public string Description { get; set; }

    public decimal TotalTransactionAmount { get; set; }

    public string Status { get; set; } = WorkOrderOutStatus.Pending;

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? UpdatedBy { get; set; }

    [Precision(0)] public DateTime? UpdatedDate { get; set; }

    [InverseProperty(nameof(WorkOrderOutItem.WorkOrderOut))]
    public virtual ICollection<WorkOrderOutItem> Items { get; set; } = new List<WorkOrderOutItem>();

    public void Approve()
    {
        if (Items.Count == 0)
            throw new InvalidWorkOrderOutItemException();

        Status = WorkOrderOutStatus.Approved;

        AddDomainEvent(new WorkOrderOutApproved(
            Code,
            WorkOrderCode,
            TransactionDate,
            Description,
            Items.Select(x => new WorkOrderOutApprovedItem(x.ItemCode, x.Quantity, x.HppAktual, x.Description))
                .ToArray()));
    }
}
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Events;

public record WorkOrderOutApprovedItem(string ItemCode, decimal Quantity, decimal Price, string Description)
    : IDomainEvent;

public record WorkOrderOutApproved(
    string Code,
    string WorkOrderCode,
    DateTime TransactionDate,
    string Description,
    WorkOrderOutApprovedItem[] Items) : IDomainEvent;
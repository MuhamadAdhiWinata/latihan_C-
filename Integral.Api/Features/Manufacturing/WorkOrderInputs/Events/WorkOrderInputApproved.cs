using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Events;

public record WorkOrderInputApprovedItem(string ItemCode, decimal Quantity, decimal Price, string Description) : IDomainEvent;

public record WorkOrderInputApproved(string Code, string WorkOrderCode, DateTime TransactionDate, string Description, WorkOrderInputApprovedItem[] Items) : IDomainEvent;
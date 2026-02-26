using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.InventoryMutations.Events;


public record MutationTransferApproved(string Code, MutationTransferApprovedDetail[] Details) : IDomainEvent;

public record MutationTransferApprovedDetail(long Id, string ItemCode, string ItemName, decimal Quantity, decimal Price, decimal SellingPrice);

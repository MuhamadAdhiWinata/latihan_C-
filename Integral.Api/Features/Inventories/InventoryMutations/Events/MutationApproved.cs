using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.InventoryMutations.Events;

public record MutationApproved(string Code) : IDomainEvent;
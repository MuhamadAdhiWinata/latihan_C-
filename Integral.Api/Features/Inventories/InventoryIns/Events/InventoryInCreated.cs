using Integral.Api.Features.Inventories.InventoryIns.Models;
using SharedKernel.Abstraction.Domain;

namespace Integral.Api.Features.Inventories.InventoryIns.Events;

public record InventoryInCreated(string Code, F303 Data) : IDomainEvent;

public record InventoryInUpdated(string Code, F303 Data) : IDomainEvent;
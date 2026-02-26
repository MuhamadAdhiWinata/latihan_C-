using SharedKernel.Abstraction;

namespace Integral.Api.Features.Inventories.InventoryIns.Exceptions;

public class InventoryInNotFoundException(string code) : AppException($"Inventory In {code} not found.");
using SharedKernel.Abstraction;

namespace Integral.Api.Features.Inventories.InventoryIns.Exceptions;

public class ItemNotFoundException(string code) : AppException(code);
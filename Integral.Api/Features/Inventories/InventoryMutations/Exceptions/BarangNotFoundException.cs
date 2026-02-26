using SharedKernel.Abstraction;

namespace Integral.Api.Features.Inventories.InventoryMutations.Exceptions;

public class BarangNotFoundException(string masterItemSku) : AppException($"Barang with SKU {masterItemSku} not found.");
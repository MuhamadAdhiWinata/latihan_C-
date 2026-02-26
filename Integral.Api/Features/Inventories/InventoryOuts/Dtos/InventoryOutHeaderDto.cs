namespace Integral.Api.Features.Inventories.InventoryOuts.Dtos;


public record InventoryOutDto(
    InventoryOutHeaderDto Header,
    IReadOnlyList<InventoryOutLineDto>? Lines
);

public record InventoryOutHeaderDto(
    string Iotno,
    string BranchCode,
    DateTime TransactionDate,
    string RefNo,
    string CurrencyCode,
    decimal ExchangeRate,
    string WarehouseCode,
    string Description
);

public record InventoryOutLineDto(
    string Iotno,
    string BranchCode,
    string ItemCode,
    string ItemName,
    decimal Quantity,
    decimal Price,
    string ReasonCode,
    string Description
);
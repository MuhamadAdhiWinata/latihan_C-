namespace Integral.Api.Features.Inventories.InventoryOuts.Dtos;

public record InventoryOutLineRequestDto
(
    string ItemCode,
    decimal Quantity,
    string ReasonCode,
    string Description
    );
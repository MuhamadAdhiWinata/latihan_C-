namespace Integral.Api.Features.Sales.SalesDeliveries.Dtos;

public record SalesDeliveryLineRequestDto(
    string ItemCode,
    string ItemAlias,
    decimal Quantity,
    string Description 
);
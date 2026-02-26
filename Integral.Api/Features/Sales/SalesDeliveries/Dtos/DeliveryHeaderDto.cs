namespace Integral.Api.Features.Sales.SalesDeliveries.Dtos;

public record DeliveryDto(
    DeliveryHeaderDto Header,
    IReadOnlyList<DeliveryLineDto>? Lines
);

public record DeliveryHeaderDto(
    string Dodno,
    string BranchCode,
    DateTime TransactionDate,
    string CurrencyCode,
    decimal ExchangeRate,
    string CustomerCode,
    string DriverCode,
    string WarehouseCode,
    decimal TotalTransaction,
    short TaxStatus,
    decimal Ppnpercent,
    decimal Ppnamount,
    decimal GrandTotal,
    short DeleteStatus,
    string Spdescription,
    string Sodno,
    bool? Approved,
    bool? Approved2
);

public record DeliveryLineDto(
    string ItemCode,
    string ItemAlias,
    decimal Quantity,
    decimal Price,
    string Description
);
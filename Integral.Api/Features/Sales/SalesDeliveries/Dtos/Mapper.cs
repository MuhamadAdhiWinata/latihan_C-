using Integral.Api.Features.Sales.SalesDeliveries.Entities;

namespace Integral.Api.Features.Sales.SalesDeliveries.Dtos;

public static class Mapper
{
    public static DeliveryHeaderDto ToDto(this SalesDelivery entity)
    {
        return new DeliveryHeaderDto(
            entity.Dodno,
            entity.BranchCode,
            entity.TransactionDate,
            entity.CurrencyCode,
            entity.ExchangeRate,
            entity.CustomerCode,
            entity.DriverCode,
            entity.WarehouseCode,
            entity.TotalTransaction,
            entity.TaxStatus,
            entity.Ppnpercent,
            entity.Ppnamount,
            entity.GrandTotal,
            entity.DeleteStatus,
            entity.Spdescription,
            entity.Sodno,
            entity.Approved,
            entity.Approved2
        );
    }

    public static DeliveryLineDto ToDto(this SalesDeliveryLine entity)
    {
        return new DeliveryLineDto(
            entity.ItemCode,
            entity.ItemAlias,
            entity.Quantity,
            entity.Price,
            entity.Description
        );
    }
}
using Integral.Api.Features.Sales.SalesOrders.Entities;

namespace Integral.Api.Features.Sales.SalesOrders.Dtos;

public record OrderProcessesDto(bool UsePrePress, bool UsePress, bool UseFinishing, bool UsePackaging);

public record OrderValueDto(decimal Percent, decimal Amount);

public record OrderHeaderDto(
    string BranchCode,
    string Sodno,
    DateTime TransactionDate,
    DateTime DeliveryDateStart,
    DateTime DeliveryDateEnd,
    DateTime ExpiredDate,
    string PaymentTermCode,
    string CustomerCode,
    string SalesmanCode,
    string CurrencyCode,
    string RefNo,
    string Description,
    bool? Approved,
    bool? Approved2
);

public record OrderCalculationDto(
    decimal TotalTransactionAmount,
    OrderValueDto Discount,
    OrderValueDto Risk,
    OrderValueDto Addon,
    OrderValueDto Margin,
    OrderValueDto Ppn,
    OrderValueDto Dpp,
    decimal FreightFeeAmount,
    decimal InsuranceFeeAmount,
    decimal AdministrationFeeAmount,
    decimal OtherFeeAmount,
    decimal FinalDiscount,
    decimal GrandTotalAmount
);

public record OrderDto(OrderHeaderDto Header, OrderCalculationDto Calculation, OrderLineItemDto[] ItemLines);

public static class Mapper
{
    public static OrderLineItemDto ToDto(this OrderItem i)
    {
        return new OrderLineItemDto(
            i.ItemCode,
            i.Description,
            i.Quantity,
            i.Price,
            i.Price * i.Quantity
        );
    }

    public static OrderHeaderDto ToDto(this Order o)
    {
        return new OrderHeaderDto(
            o.BranchCode,
            o.Sodno,
            o.TransactionDate,
            o.DeliveryDateStart,
            o.DeliveryDateEnd,
            o.ExpiredDate,
            o.PaymentTermCode,
            o.CustomerCode,
            o.SalesmanCode,
            o.CurrencyCode,
            o.RefNo,
            o.Description,
            o.Approved,
            o.Approved2);
    }

    public static OrderDto ToDto(this Order o, OrderItem[] items)
    {
        var header = new OrderHeaderDto(
            o.BranchCode,
            o.Sodno,
            o.TransactionDate,
            o.DeliveryDateStart,
            o.DeliveryDateEnd,
            o.ExpiredDate,
            o.PaymentTermCode,
            o.CustomerCode,
            o.SalesmanCode,
            o.CurrencyCode,
            o.RefNo,
            o.Description,
            o.Approved,
            o.Approved2);

        var calculation = new OrderCalculationDto(
            o.TotalTransactionAmount,
            new OrderValueDto(o.DiscountPercent, o.DiscountAmount),
            new OrderValueDto(o.RiskPercent, o.RiskAmount),
            new OrderValueDto(o.AddPercent, o.AddAmount),
            new OrderValueDto(o.MarginPercent, o.MarginAmount),
            new OrderValueDto(o.Ppnpercent, o.Ppnamount),
            new OrderValueDto(o.DppPercent, o.Dppamount),
            o.FreightFeeAmount,
            o.InsuranceFeeAmount,
            o.AdministrationFeeAmount,
            o.OtherFeeAmount,
            o.FinalDiscount,
            o.GrandTotalAmount
        );

        return new OrderDto(header, calculation, items.Select(x => x.ToDto()).ToArray());
    }
}

public record OrderLineItemDto(
    string ItemCode,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalAmount
);
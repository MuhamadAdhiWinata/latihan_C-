using Integral.Api.Features.Sales.SalesInvoices.Entities;

namespace Integral.Api.Features.Sales.SalesInvoices.Dtos;

public record InvoiceLineDto(string ItemCode, string ItemAlias, decimal Quantity, decimal Price, decimal Total );

public record InvoiceDeliveryDto(string Code);

public record InvoiceHeaderDto(
    string Code,
    DateTime TransactionDate,
    string CustomerCode,
    string CustomerName,
    string CurrencyCode,
    decimal ExchangeRate,
    string Description,
    string SalesOrderNo,
    
    decimal TotalTransaction, 
    decimal DiscountPercent, 
    decimal DiscountAmount,
    decimal PpnPercent,
    decimal PpnEffectivePercent,
    decimal PpnAmount, 
    decimal DppAmount,
    decimal DeliveryFeeBeforePpn,
    decimal GrandTotal
);

public static class Mapper
{
    public static InvoiceLineDto ToDto(this SalesInvoiceLine line)
    {
        return new InvoiceLineDto(line.ItemCode, line.ItemAlias, line.Quantity, line.Price, line.Total);
    }
    
    public static InvoiceDeliveryDto ToDto(this SalesInvoiceDelivery delivery)
    {
        return new InvoiceDeliveryDto(delivery.Dodno);
    }

    public static InvoiceHeaderDto ToDto(this SalesInvoice inv)
    {
        return new InvoiceHeaderDto(
            inv.Invno,
            inv.TransactionDate,
            inv.CustomerCode,
            "",
            inv.CurrencyCode,
            inv.ExchangeRate,
            inv.Description,
            inv.Sodno,
            
            inv.TotalTransaction,
            inv.DiscountPercent, 
            inv.DiscountAmount,
            inv.Ppnpercent, 
            inv.PpneffectivePercent,
            inv.Ppnamount, 
            inv.Dppamount,
            inv.DeliveryFeeBeforePpn, 
            inv.GrandTotal);
    }
}
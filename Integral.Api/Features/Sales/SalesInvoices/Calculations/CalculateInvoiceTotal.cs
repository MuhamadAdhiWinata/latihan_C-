using Integral.Api.Features.Sales.SalesInvoices.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesInvoices.Calculations;

public record CalculateInvoiceTotal(
    decimal TotalTransaction, decimal DiscountAmount, 
    decimal PpnPercent, decimal PpnEffectivePercent,
    decimal DeliveryFeeBeforePpn
) : IQuery<CalculateInvoiceTotalResult>
{
    
}

public record CalculateInvoiceTotalResult(
    decimal TotalTransaction, 
    decimal DiscountPercent, 
    decimal DiscountAmount,
    decimal PpnPercent,
    decimal PpnEffectivePercent,
    decimal PpnAmount, 
    decimal DppAmount,
    decimal DeliveryFeeBeforePpn,
    decimal GrandTotal
)
{
}

public class CalculateInvoiceTotalHandler() : IQueryHandler<CalculateInvoiceTotal, CalculateInvoiceTotalResult>
{
    public Task<CalculateInvoiceTotalResult> Handle(CalculateInvoiceTotal request, CancellationToken cancellationToken)
    {
        var invoiceCalc = new SalesInvoice()
        {
            TotalTransaction = request.TotalTransaction,
            DiscountAmount = request.DiscountAmount,
            Ppnpercent = request.PpnPercent,
            PpneffectivePercent = request.PpnEffectivePercent,
            DeliveryFeeBeforePpn = request.DeliveryFeeBeforePpn,
        };
        
        invoiceCalc.Calculate();
        return Task.FromResult(new CalculateInvoiceTotalResult(
            invoiceCalc.TotalTransaction, 
            invoiceCalc.DiscountPercent, invoiceCalc.DiscountAmount, 
            invoiceCalc.Ppnpercent, invoiceCalc.PpneffectivePercent, 
            invoiceCalc.Ppnamount, invoiceCalc.Dppamount,
            invoiceCalc.DeliveryFeeBeforePpn, invoiceCalc.GrandTotal));
    }
}

public class CalculateInvoiceTotalEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{SalesInvoiceRoot.ApiPath}/calculate-totals", Handle)
            .RequireAuthorization()
            .WithTags($"{SalesInvoiceRoot.Title}")
            .WithName($"Calculate {SalesInvoiceRoot.Title} Totals");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CalculateInvoiceTotal request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
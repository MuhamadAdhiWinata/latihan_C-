using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesInvoices.Dtos;
using Integral.Api.Features.Sales.SalesInvoices.Entities;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesInvoices.Calculations;

public record CalculateInvoiceDeliveries(string[] DeliveryCodes) : IQuery<CalculateInvoiceDeliveriesResult>
{
}

public record CalculateInvoiceDeliveriesResult(InvoiceDeliveryDto[] Deliveries, InvoiceLineDto[] Lines, decimal Total)
{
}

public class CalculateInvoiceDeliveriesHandler(PrintingDbContext dbContext) : IQueryHandler<CalculateInvoiceDeliveries, CalculateInvoiceDeliveriesResult>
{
    public async Task<CalculateInvoiceDeliveriesResult> Handle(CalculateInvoiceDeliveries request, CancellationToken cancellationToken)
    {
        var deliveries = await dbContext.SalesDeliveries
            .AsNoTracking()
            .Include(x => x.F606s)
            .Where(x => request.DeliveryCodes.Contains(x.Dodno))
            .ToListAsync(cancellationToken);

        var invoiceCalc = new SalesInvoice();

        foreach (var deliveryLine in deliveries)
        {
            invoiceCalc.AddDelivery(deliveryLine);
        }

        return new CalculateInvoiceDeliveriesResult(
            invoiceCalc.F928s.Select(x => x.ToDto()).ToArray(), 
            invoiceCalc.F608s.Select(x => x.ToDto()).ToArray(),
            invoiceCalc.F608s.Sum(x => x.Sum()));
    }
}

public class CalculateInvoiceDeliveriesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{SalesInvoiceRoot.ApiPath}/calculate-deliveries", Handle)
            .RequireAuthorization()
            .RequirePermission("sales.delivery.read")
            .WithTags($"{SalesInvoiceRoot.Title}")
            .WithName($"Calculate {SalesInvoiceRoot.Title} Deliveries");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CalculateInvoiceDeliveries request)
    {
        var res = await mediator.Send(request);
        return Results.Ok();
    }
}

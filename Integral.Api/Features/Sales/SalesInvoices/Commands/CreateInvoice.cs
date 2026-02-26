using Integral.Api.Features.Sales.SalesInvoices.Dtos;
using SharedKernel.Abstraction.CQRS;

namespace Integral.Api.Features.Sales.SalesInvoices.Commands;

public record CreateInvoice(string[] DeliveryCodes) : ICommand<CreateInvoiceResult>
{
}

public record CreateInvoiceResult(InvoiceDeliveryDto[] Deliveries, InvoiceLineDto[] Lines)
{
}

// public class CreateInvoiceHandler(PrintingDbContext dbContext) : ICommandHandler<CreateInvoice, CreateInvoiceResult>
// {
//     public async Task<CreateInvoiceResult> Handle(CreateInvoice request, CancellationToken cancellationToken)
//     {
//
//     }
// }
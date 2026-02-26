using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesInvoices.Dtos;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesInvoices.Queries;
public record FindAllInvoices(bool? IsApproved, DateTime? StartDate, DateTime? EndDate, int Limit = 100, int Offset = 0, string Search = "") : IQuery<FindAllInvoicesResult>
{
}

public record FindAllInvoicesResult(int Count, InvoiceHeaderDto[] Data) {}

public class FindAllInvoicesHandler(PrintingDbContext dbContext) : IQueryHandler<FindAllInvoices, FindAllInvoicesResult>
{
    public async Task<FindAllInvoicesResult> Handle(FindAllInvoices request, CancellationToken cancellationToken)
    {
        var query = dbContext.SalesInvoices
            .OrderByDescending(x => x.Invno)
            .AsNoTracking();

        if (request.IsApproved.HasValue)
            query = query.Where(x => x.PostingRec > 0 == request.IsApproved.Value);
        
        if (request.StartDate != null)
            query = query.Where(x => x.TransactionDate >= request.StartDate);
        
        if (request.EndDate != null)
            query = query.Where(x => x.TransactionDate <= request.EndDate);
            
        var invoices = await query
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);
        
        return new FindAllInvoicesResult(await query.CountAsync(cancellationToken), invoices.ToArray());
    }
}

public class FindAllInvoicesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{SalesInvoiceRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .RequirePermission($"{SalesInvoiceRoot.Slug}.read")
            .WithTags($"{SalesInvoiceRoot.Title}")
            .WithName($"Find All {SalesInvoiceRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllInvoices request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
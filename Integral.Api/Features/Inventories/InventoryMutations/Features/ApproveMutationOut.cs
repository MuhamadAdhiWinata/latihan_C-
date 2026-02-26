using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Events;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Features;

public record ApproveMutationOut(string Code) : ICommand<ApproveMutationTransferResult>;

public record ApproveMutationTransferResult()
{
}

public class ApproveMutationTransferHandler(PrintingDbContext dbContext, IMediator mediator) : ICommandHandler<ApproveMutationOut, ApproveMutationTransferResult>
{
    public async Task<ApproveMutationTransferResult> Handle(ApproveMutationOut request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.MutationEntries
            .Include(x => x.MutationItems)
            .ThenInclude(x => x.ItemNavigation)
            .Where(x => x.TransactionCode == request.Code)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entry == null) 
            throw new AppException($"Mutation Entry {request.Code} not found.");

        if (entry.Status == MutationStatus.APPROVED)
            throw new AppException("Mutation Entry already approved.");
        
        var stocks = await dbContext.StockTransactions
            .Include(x => x.ReversedBy)
            .Include(x => x.ReversalOf)
            .Include(x => x.StockBatch)
            .ThenInclude(x => x.Transactions)
            .Where(x => x.RefCode == request.Code)
            .Where(y => y.ReversedBy == null)
            .ToListAsync(cancellationToken);

        var lines = new List<MutationTransferApprovedDetail>();

        foreach (var item in entry.MutationItems)
        {
            var relatedStocks = stocks
                .Where(x => x.StockBatch.ItemCode == item.ItemCode && x.Out > 0)
                .ToList();

            foreach (var stock in relatedStocks)
            {
                lines.Add(new MutationTransferApprovedDetail(
                    item.Id,
                    item.ItemCode,
                    item.ItemName,
                    stock.Out,
                    stock.StockBatch.Cogsidr,
                    item.SellingPrice
                ));
            }
        }
        
        // entry.Approve();
        entry.Status = MutationStatus.APPROVED;
        entry.AddDomainEvent(new MutationTransferApproved(entry.TransactionCode, lines.ToArray()));
        
        return new ApproveMutationTransferResult();
    }
}

public class ApproveWorkOrderOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{InventoryMutationRoot.ApiPath}/{{code}}/approve", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Approve {InventoryMutationRoot.Title}");
        
        
        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new ApproveMutationOut(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
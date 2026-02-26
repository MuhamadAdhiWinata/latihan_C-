using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Features;

public record DeleteMutationOut(string Code) : ICommand<DeleteMutationTransferResult>
{
}

public record DeleteMutationTransferResult()
{
}

public class DeleteMutationTransferHandler(PrintingDbContext printingDb) : ICommandHandler<DeleteMutationOut,  DeleteMutationTransferResult>
{
    public async Task<DeleteMutationTransferResult> Handle(DeleteMutationOut request, CancellationToken cancellationToken)
    {
        var entry = await printingDb.MutationEntries
            .Include(x => x.MutationItems)
            .Where(x => x.TransactionCode == request.Code)
            .FirstOrDefaultAsync(cancellationToken);
        
        var transactions = await printingDb.StockTransactions
            .Where(x => x.RefCode == entry.TransactionCode)
            .ToListAsync(cancellationToken);
        
        foreach (var transaction in transactions)
        {
            var stock = await printingDb.StockBatches.Where(x => x.Id == transaction.StockId).FirstOrDefaultAsync(cancellationToken);
            stock.UsedStock -= transaction.Out;
            
            printingDb.StockTransactions.Remove(transaction);
        }
        
        printingDb.MutationEntries.Remove(entry);

        return new DeleteMutationTransferResult();
    }
}

public class DeleteMutationTransferEndpoint() : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{InventoryMutationRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Delete {InventoryMutationRoot.Title}");
        
        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var command = new DeleteMutationOut(code);
        var res = await mediator.Send(command);
        
        return Results.Ok(res);
    }

}
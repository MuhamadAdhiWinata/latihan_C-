using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Queries;

public record FindByCodeMutationOld
    (string Code) : ICommand<FindByCodeMutationOldResult>
{
}

public record FindByCodeMutationOldResult(MutationTransferSkuDto Data, MutationTransferItemSkuDto[] Items)
{
}

public class FindByCodeMutationOldHandler(PrintingDbContext dbContext, IMediator mediator) : ICommandHandler<FindByCodeMutationOld
    , FindByCodeMutationOldResult>
{
    public async Task<FindByCodeMutationOldResult> Handle(FindByCodeMutationOld
            request, CancellationToken cancellationToken)
    {        
        var entry = await dbContext.MutationEntries
            .Include(x => x.MutationItems)
            .Where(x => x.TransactionCode == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        var itemCodes = entry.MutationItems.Select(i => i.ItemCode).ToArray();
        
        var masterItems = await dbContext.Items
            .Where(x => itemCodes.Contains(x.Code))
            .ToDictionaryAsync(x => x.Code, cancellationToken);

        var itemTraces = await dbContext.StockTransactions
            .Include(x => x.StockBatch)
            .Where(x => x.RefCode == request.Code)
            .Select(x => new
            {
                Outward = x,
                Stock = x.StockBatch,
                Inward = dbContext.StockTransactions
                    .Include(i => i.StockBatch)
                    .FirstOrDefault(i => i.StockId == x.StockId &&
                                         i.In > 0 &&
                                         i.RefCode!.StartsWith("IIN")),
            })
            .ToListAsync(cancellationToken);
        
        var header = new MutationTransferSkuDto(
            entry.Id,
            entry.TransactionCode,
            entry.TransactionDate,
            entry.RefNo ?? "",
            entry.Description);
        
        var items = entry.MutationItems
            .Select(x =>
            {
                masterItems.TryGetValue(x.ItemCode, out var master);

                var trace = itemTraces.FirstOrDefault(
                        y => 
                            y.Outward.StockBatch.ItemCode == x.ItemCode && 
                            y.Outward.Out == x.Quantity);


                var refNo = "";
                
                if (trace?.Inward != null)
                {
                    var iin = trace.Inward.RefCode;
                    var inventoryIn = dbContext.F303s.FirstOrDefault(x => x.Iinno == iin);
                        
                    refNo = inventoryIn?.RefNo ?? "";
                }
                
                return new MutationTransferItemSkuDto(
                    x.ItemCode,
                    master?.Name,
                    master?.Sku,
                    refNo,
                    x.Quantity,
                    x.Price,
                    x.SellingPrice
                );
            })
            .ToArray();
        
        
        return new FindByCodeMutationOldResult(header, items);
    }
}

public record MutationTransferItemSkuDto(string ArgItemCode, string? MasterName, string? MasterSku, string RefNo, decimal ArgQuantity, decimal ArgPrice, decimal ArgSellingPrice);

public record MutationTransferSkuDto(int EntryId, string EntryTransactionCode, DateTime EntryTransactionDate, string EntryRefNo, string EntryDescription);

public class FindByCodeMutationOldEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        // builder.MapGet($"{ InventoryRoot.BaseApiPath}/merger/deliveries/{{code}}", Handle)
        //     .RequireAuthorization()
        //     .WithTags(DeprecationRoot.DeprecatedTag)
        //     .WithName("Find Inventory Mutation By Code");
        
        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeMutationOld(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
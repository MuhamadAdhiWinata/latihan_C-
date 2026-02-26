using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Queries;

public record FindByCodeMutation(string Code) : IQuery<FindByCodeMutationResult>;

public record FindByCodeMutationResult(MutationDto Data);

public class FindByCodeMutationHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindByCodeMutation, FindByCodeMutationResult>
{
    public async Task<FindByCodeMutationResult> Handle(FindByCodeMutation request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.MutationEntries
            .AsNoTracking()
            .Include(x => x.MutationItems)
            .ThenInclude(x => x.ItemNavigation)
            .Where(x => x.TransactionCode == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (entry == null) throw new AppException($"Mutation entry with code '{request.Code}' was not found.");

        var outTransactions = await dbContext.StockTransactions
            .AsNoTracking()
            .Where(x => x.RefCode == request.Code)
            .Where(st => !dbContext.StockTransactions.Any(r => r.ReverseOfId == st.Id))
            .Include(stockTransaction => stockTransaction.StockBatch)
            .ToListAsync(cancellationToken);

        var batchIds = outTransactions.Select(x => x.StockId).ToList();

        var inTransactions = await dbContext.StockTransactions
            .AsNoTracking()
            .Where(x => batchIds.Contains(x.StockId))
            .Where(x => x.ReverseOfId == null)
            .Where(x => x.In > 0)
            .ToListAsync(cancellationToken);

        var entries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tx in inTransactions)
        {
            if (tx.In > 0 && !string.IsNullOrEmpty(tx.RefCode) && tx.ReverseOfId == null)
                entries.Add(tx.RefCode);
        }

        var inventoryIns = await dbContext.F303s
            .Where(x => entries.Contains(x.Iinno))
            .ToListAsync(cancellationToken);

        var author = await dbContext.Users.FirstOrDefaultAsync(x => x.Code == entry.CreatedBy, cancellationToken);

        var res = new MutationDto(
            entry.ToDto(author.Name),
            entry.MutationItems.Select(x => x.ToDto(outTransactions
                .Where(y => y.RefCode == entry.TransactionCode)
                .Where(y => y.StockBatch.ItemCode == x.ItemCode)
                .Where(y => y.ReverseOfId == null)
                .Where(y => y.Out > 0)
                .ToArray()
            )).ToArray(),
            inventoryIns
                .Select(x => x.RefNo)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
        );

        return new FindByCodeMutationResult(res);
    }
}

public class FindByCodeMutationEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryMutationRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Find By Code {InventoryMutationRoot.Title}");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeMutation(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
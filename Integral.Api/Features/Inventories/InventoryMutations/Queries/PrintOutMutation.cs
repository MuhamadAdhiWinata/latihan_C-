using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Queries;

public record PrintOutMutationLineDto(
    string ItemCode,
    string Sku,
    decimal SellingPrice,
    decimal Quantity,
    decimal Cost
);

public record PrintOutMutation(string Code) : IQuery<PrintOutMutationResult>
{
}

public record PrintOutMutationResult(object Data)
{
}

public class PrintOutMutationHandler(PrintingDbContext dbContext) 
    : IQueryHandler<PrintOutMutation, PrintOutMutationResult>
{
    public async Task<PrintOutMutationResult> Handle(PrintOutMutation request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.MutationEntries
            .AsNoTracking()
            .Include(x => x.MutationItems)
            .ThenInclude(x => x.ItemNavigation)
            .Where(x => x.TransactionCode == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (entry == null) 
            throw new AppException($"Mutation entry with code '{request.Code}' was not found.");
        
        var stocks = await dbContext.StockTransactions
            .Include(x => x.ReversedBy)
            .Include(x => x.ReversalOf)
            .Include(x => x.StockBatch)
                .ThenInclude(x => x.Transactions)
            .Where(x => x.RefCode == request.Code)
            .Where(y => y.ReversedBy == null)
            .ToListAsync(cancellationToken);

        var lines = new List<PrintOutMutationLineDto>();

        foreach (var item in entry.MutationItems)
        {
            var relatedStocks = stocks
                .Where(x => x.StockBatch.ItemCode == item.ItemCode && x.Out > 0)
                .ToList();

            foreach (var stock in relatedStocks)
            {
                lines.Add(new PrintOutMutationLineDto(
                    item.ItemCode,
                    item.ItemNavigation.Sku,
                    item.SellingPrice,
                    stock.Out,
                    stock.StockBatch.Cogsidr
                ));
            }
        }

        var author = await dbContext.Users.FirstOrDefaultAsync(x => x.Code == entry.CreatedBy, cancellationToken);
        
        var result = new PrintOutMutationResult(
            new
            {
                header = entry.ToDto(author.Name),
                lines = lines
            }
        );

        return result;
    }
}


public class PrintOutMutationEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryMutationRoot.ApiPath}/{{code}}/printout", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Printout {InventoryMutationRoot.Title}");
        
        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new PrintOutMutation(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
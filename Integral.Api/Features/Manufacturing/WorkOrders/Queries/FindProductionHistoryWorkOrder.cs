using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Queries;

public record FindProductionHistoryWorkOrder(string WorkOrderCode) : IQuery<FindHistoryWorkOrderOutResult>;

public record FindHistoryWorkOrderOutResult(WorkOrderOutDto[] Data);

public class FindHistoryWorkOrderOutHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindProductionHistoryWorkOrder, FindHistoryWorkOrderOutResult>
{
    public async Task<FindHistoryWorkOrderOutResult> Handle(
        FindProductionHistoryWorkOrder request,
        CancellationToken cancellationToken)
    {
        var wo = await dbContext.WorkOrders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Dodno == request.WorkOrderCode, cancellationToken);

        var query =
            dbContext.WorkOrderOuts
                .AsNoTracking()
                .Include(x => x.Items)
                .ThenInclude(x => x.ItemNavigation)
                .Where(x => x.WorkOrderCode == request.WorkOrderCode)
                .OrderBy(x => x.TransactionDate)
                .ThenBy(x => x.Code)
                .Select(x => new WorkOrderOutDto(x.ToDto(), x.Items.Select(i => i.ToDto()).ToArray()));

        var result = await query.ToListAsync(cancellationToken);

        if (result.Count == 0)
            return new FindHistoryWorkOrderOutResult([]);

        var remaining = wo.Items.ToDictionary(g => g.ItemCode, g => g.Quantity);

        foreach (var woOut in result)
        foreach (var item in woOut.Lines)
        {
            var remainingBefore = remaining[item.ItemCode];
            remaining[item.ItemCode] = remainingBefore - item.Quantity;

            if (remainingBefore < 0)
                remainingBefore = 0;

            item.UnfinishedQuantity = remainingBefore;
        }

        return new FindHistoryWorkOrderOutResult(result.ToArray());
    }
}

public class FindHistoryWorkOrderOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderRoot.ApiPath}/{{code}}/history", Handle)
            .RequireAuthorization()
            .WithTags(WorkOrderRoot.Tag)
            .WithName($"Find {WorkOrderRoot.Title} History");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindProductionHistoryWorkOrder(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
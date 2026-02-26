using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Features;

public record CloseWorkOrder(string Code) : ICommand<CloseWorkOrderResult>
{
}

public record CloseWorkOrderResult
{
}

public class CloswWorkOrderHandler(PrintingDbContext dbContext) : ICommandHandler<CloseWorkOrder, CloseWorkOrderResult>
{
    public async Task<CloseWorkOrderResult> Handle(CloseWorkOrder request, CancellationToken cancellationToken)
    {
        var workOrder = await dbContext.WorkOrders.Where(x => x.Dodno == request.Code).FirstOrDefaultAsync(cancellationToken);
        if (workOrder == null)
            throw new Exception("Work order not found");
        
        var woItems = await dbContext.WorkOrderItems.Where(x => x.Dodno == request.Code).ToListAsync(cancellationToken);
        
        var processedItems = await dbContext.WorkOrderOutItems
            .Include(x => x.WorkOrderOut)
            .Where(x => x.WorkOrderOut.WorkOrderCode == request.Code)
            .ToListAsync(cancellationToken);
        
        var unprocessedItems = woItems
            .Where(item => !processedItems.Any(p => p.ItemCode == item.ItemCode))
            .ToList();

        if (unprocessedItems.Any())
        {
            var missing = string.Join(", ", unprocessedItems.Select(x => x.ItemCode));
            throw new Exception($"Cannot close work order. Unprocessed items: {missing}");
        }
        
        workOrder.Close();
        
        return new CloseWorkOrderResult();
    }
}

public class ApproveWorkOrderOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        // builder.MapPost($"{WorkOrderRoot.DeprecatedApiPath}/spk/{{code}}/close", Handle)
        //     .RequireAuthorization()
        //     .WithTags(DeprecationRoot.DeprecatedTag)
        //     .WithName("Close SPK");
        
        builder.MapPost($"{WorkOrderRoot.ApiPath}/{{code}}/close", Handle)
            .RequireAuthorization()
            .WithTags(WorkOrderRoot.Tag)
            .WithName($"Close {WorkOrderRoot.Title}");
        
        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new CloseWorkOrder(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
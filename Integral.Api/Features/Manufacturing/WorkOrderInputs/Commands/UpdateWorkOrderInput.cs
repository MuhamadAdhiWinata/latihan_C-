using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Commands;

public record UpdateWorkOrderInput(
    string Code,
    DateTime TransactionDate,
    string WorkOrderCode,
    UpdateWorkOrderInputItem[] Items,
    string Description = "") : ICommand<UpdateWorkOrderInputResult>
{
}

public record UpdateWorkOrderInputItem(
    string ItemCode,
    string UnitCode,
    decimal Quantity,
    decimal Price,
    decimal SellingPrice,
    string Description);

public class UpdateWorkOrderInputResult
{
}

public class UpdateWorkOrderInputHandler(PrintingDbContext dbContext)
    : ICommandHandler<UpdateWorkOrderInput, UpdateWorkOrderInputResult>
{
    public async Task<UpdateWorkOrderInputResult> Handle(UpdateWorkOrderInput request,
        CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderInputs
            .Include(x => x.Items)
            .Where(x => x.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (entry == null) throw new Exception("Work Order out not found");

        entry.Items.Clear();
        
        entry.Description = request.Description;
        entry.TransactionDate = request.TransactionDate;
        entry.TotalTransactionAmount = 0;

        foreach (var item in request.Items)
        {
            entry.Items.Add(new WorkOrderInputItem()
            {
                ItemCode = item.ItemCode,
                Price = item.Price,
                Quantity = item.Quantity,
                Description = item.Description,
            });

            entry.TotalTransactionAmount += item.Quantity * item.SellingPrice;
        }

        return new UpdateWorkOrderInputResult();
    }
}

public class UpdateWorkOrderInputItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{WorkOrderInputRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderInputRoot.Tag}")
            .WithName($"Update {WorkOrderInputRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] UpdateWorkOrderInputEndpointBody request,
        string code)
    {
        var command = new UpdateWorkOrderInput(code, request.TransactionDate, request.WorkOrderCode, request.Items,
            request.Description);
        var res = await mediator.Send(command);

        return Results.Ok(res);
    }

    private record UpdateWorkOrderInputEndpointBody(
        DateTime TransactionDate,
        string WorkOrderCode,
        UpdateWorkOrderInputItem[] Items,
        string Description);
}
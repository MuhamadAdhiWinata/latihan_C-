using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;
using Integral.Api.Features.Manufacturing.WorkOrders.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Commands;

public record CreateWorkOrderInput(
    DateTime TransactionDate,
    string WorkOrderCode,
    CreateWorkOrderInputItem[] Items,
    string Description = "") : ICommand<CreateWorkOrderInputResult>;

public record CreateWorkOrderInputItem(
    string ItemCode,
    string UnitCode,
    decimal Quantity,
    decimal Price,
    string Description);

public record CreateWorkOrderInputResult();

public class CreateWorkOrderInputHandler(PrintingDbContext dbContext, CurrentUserProvider currentUser)
    : ICommandHandler<CreateWorkOrderInput, CreateWorkOrderInputResult>
{
    public async Task<CreateWorkOrderInputResult> Handle(CreateWorkOrderInput request,
        CancellationToken cancellationToken)
    {
        var codegen = new CodeGenerator(dbContext);

        var wo = dbContext.WorkOrders.FirstOrDefault(x => x.Dodno == request.WorkOrderCode);
        if (wo == null)
            throw new Exception($"Work order code {request.WorkOrderCode} not found");

        if (wo.Status == WorkOrderStatus.Finished)
            throw new Exception($"Work order code {request.WorkOrderCode} already finished");

        var entry = new WorkOrderInput()
        {
            Code = await codegen.GenerateAsync<WorkOrderInput>("WOIN", nameof(WorkOrderInput.Code)),
            TransactionDate = request.TransactionDate,
            WorkOrderCode = request.WorkOrderCode,
            Description = request.Description,
            Status = WorkOrderInputStatus.Submitted,
            CreatedBy = currentUser.GetUsername()
        };

        foreach (var item in request.Items)
        {
            entry.Items.Add(new WorkOrderInputItem()
            {
                ItemCode = item.ItemCode,
                Price = item.Price,
                Quantity = item.Quantity,
                Description = item.Description,
            });

            entry.TotalTransactionAmount += item.Quantity * item.Price;
        }

        await dbContext.WorkOrderInputs.AddAsync(entry, cancellationToken);

        return new CreateWorkOrderInputResult();
    }
}

public class CreateInternalDeliveryController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{WorkOrderInputRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderInputRoot.Tag}")
            .WithName($"Create {WorkOrderInputRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateWorkOrderInput request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
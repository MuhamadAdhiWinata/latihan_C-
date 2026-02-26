using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Models;
using Integral.Api.Features.Manufacturing.WorkOrders.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderOutputs.Commands;

public record CreateWorkOrderOut(
    DateTime TransactionDate,
    string WorkOrderCode,
    CreateWorkOrderOutItem[] Items,
    string Description = "") : ICommand<CreateWorkOrderOutResult>;

public record CreateWorkOrderOutItem(
    string ItemCode,
    string UnitCode,
    decimal Quantity,
    decimal HppEstimasi,
    decimal Hpp,
    string Description);

public record CreateWorkOrderOutResult();

public class CreateWorkOrderOutHandler(PrintingDbContext dbContext, CurrentUserProvider currentUser)
    : ICommandHandler<CreateWorkOrderOut, CreateWorkOrderOutResult>
{
    public async Task<CreateWorkOrderOutResult> Handle(CreateWorkOrderOut request, CancellationToken cancellationToken)
    {
        var codegen = new CodeGenerator(dbContext);

        var wo = dbContext.WorkOrders.FirstOrDefault(x => x.Dodno == request.WorkOrderCode);
        if (wo == null)
            throw new Exception($"Work order code {request.WorkOrderCode} not found");

        if (wo.Status == WorkOrderStatus.Finished)
            throw new Exception($"Work order code {request.WorkOrderCode} already finished");

        var entry = new WorkOrderOut()
        {
            Code = await codegen.GenerateAsync<WorkOrderOut>("WOF", nameof(WorkOrderOut.Code)),
            TransactionDate = request.TransactionDate,
            WorkOrderCode = request.WorkOrderCode,
            Description = request.Description,
            Status = WorkOrderOutStatus.Submitted,
            CreatedBy = currentUser.GetUsername()
        };

        foreach (var item in request.Items)
        {
            entry.Items.Add(new WorkOrderOutItem()
            {
                ItemCode = item.ItemCode,
                Price = item.HppEstimasi,
                HppAktual = item.Hpp,
                Quantity = item.Quantity,
                Description = item.Description,
            });

            entry.TotalTransactionAmount += item.Quantity * item.Hpp;
        }

        await dbContext.WorkOrderOuts.AddAsync(entry, cancellationToken);

        return new CreateWorkOrderOutResult();
    }
}

public class CreateInternalDeliveryController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{WorkOrderOutRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderOutRoot.Tag}")
            .WithName($"Create {WorkOrderOutRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateWorkOrderOut request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
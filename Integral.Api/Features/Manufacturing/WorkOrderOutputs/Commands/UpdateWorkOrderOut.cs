using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Commands;

public record UpdateWorkOrderOut( string Code, DateTime TransactionDate, string WorkOrderCode, UpdateWorkOrderOutItem[] Items, string Description = "") : ICommand<UpdateWorkOrderOutResult> 
{
}

public record UpdateWorkOrderOutItem(
    string ItemCode,
    string UnitCode,
    decimal Quantity,
    decimal HppEstimasi,
    decimal Hpp,
    string Description);

public class UpdateWorkOrderOutResult
{
}

public class UpdateWorkOrderOutHandler(PrintingDbContext dbContext) : ICommandHandler<UpdateWorkOrderOut, UpdateWorkOrderOutResult>
{
    public async Task<UpdateWorkOrderOutResult> Handle(UpdateWorkOrderOut request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderOuts
            .Include(x => x.Items)
            .Where(x => x.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entry == null) throw new Exception("Work Order out not found");
        
        entry.Items.Clear();
        entry.TotalTransactionAmount = 0;

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

        return new UpdateWorkOrderOutResult();
    }
}

public class UpdateWorkOrderOutItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{WorkOrderOutRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderOutRoot.Tag}")
            .WithName($"Update {WorkOrderOutRoot.Title}");
        
        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] UpdateWorkOrderOutEndpointBody request,
        string code)
    {
        var command = new UpdateWorkOrderOut(code, request.TransactionDate, request.WorkOrderCode, request.Items, request.Description);
        var res = await mediator.Send(command);
        
        return Results.Ok(res);
    }

    private record UpdateWorkOrderOutEndpointBody(
        DateTime TransactionDate, string WorkOrderCode, UpdateWorkOrderOutItem[] Items, string Description);
}
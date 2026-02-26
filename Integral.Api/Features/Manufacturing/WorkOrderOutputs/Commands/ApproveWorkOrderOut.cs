using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Commands;

public record ApproveWorkOrderOut(string Code) : ICommand;

public class ApproveWorkOrderOutHandler(PrintingDbContext dbContext) : ICommandHandler<ApproveWorkOrderOut>
{
    public async Task<Unit> Handle(ApproveWorkOrderOut request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderOuts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);
        if (entry == null)
            throw new WorkOrderOutNotFoundException(request.Code);
        
        entry.Approve();
        
        return Unit.Value;
    }
}

public class ApproveWorkOrderOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{WorkOrderOutRoot.ApiPath}/{{code}}/approve", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderOutRoot.Tag}")
            .WithName($"Approve {WorkOrderOutRoot.Title}");
        
        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new ApproveWorkOrderOut(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
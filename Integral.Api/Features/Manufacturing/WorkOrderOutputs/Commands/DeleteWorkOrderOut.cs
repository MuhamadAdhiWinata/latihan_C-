using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Commands;

public record DeleteWorkOrderOut(string Code) : ICommand<DeleteWorkOrderOutResult>
{
}

public record DeleteWorkOrderOutResult
{
}

public class DelteWorkOrderOutHandler(PrintingDbContext dbContext) : IRequestHandler<DeleteWorkOrderOut, DeleteWorkOrderOutResult>
{
    public async Task<DeleteWorkOrderOutResult> Handle(DeleteWorkOrderOut request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderOuts
            .Include(x => x.Items)
            .Where(x => x.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);
        
        dbContext.Remove(entry);
        return new DeleteWorkOrderOutResult();
    }
}

public class DeleteWorkOrderOutHandler() : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{WorkOrderOutRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderOutRoot.Tag}")
            .WithName($"Delete {WorkOrderOutRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var command = new DeleteWorkOrderOut(code);
        var res = await mediator.Send(command);
        
        return Results.Ok(res);
    }
}

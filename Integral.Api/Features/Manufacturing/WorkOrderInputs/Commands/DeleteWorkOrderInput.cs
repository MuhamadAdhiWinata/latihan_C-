using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Commands;

public record DeleteWorkOrderInput(string Code) : ICommand<DeleteWorkOrderInputResult>
{
}

public record DeleteWorkOrderInputResult
{
}

public class DelteWorkOrderInputHandler(PrintingDbContext dbContext) : IRequestHandler<DeleteWorkOrderInput, DeleteWorkOrderInputResult>
{
    public async Task<DeleteWorkOrderInputResult> Handle(DeleteWorkOrderInput request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderInputs
            .Include(x => x.Items)
            .Where(x => x.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);
        
        dbContext.Remove(entry);
        return new DeleteWorkOrderInputResult();
    }
}

public class DeleteWorkOrderInputHandler() : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{WorkOrderInputRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderInputRoot.Tag}")
            .WithName($"Delete {WorkOrderInputRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var command = new DeleteWorkOrderInput(code);
        var res = await mediator.Send(command);
        
        return Results.Ok(res);
    }
}

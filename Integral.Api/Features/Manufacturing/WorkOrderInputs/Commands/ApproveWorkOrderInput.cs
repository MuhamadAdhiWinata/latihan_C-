using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Commands;

public record ApproveWorkOrderInput(string Code) : ICommand;

public class ApproveWorkOrderInputHandler(PrintingDbContext dbContext) : ICommandHandler<ApproveWorkOrderInput>
{
    public async Task<Unit> Handle(ApproveWorkOrderInput request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderInputs
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);
        if (entry == null)
            throw new DomainRuleException(request.Code);

        entry.Approve();

        return Unit.Value;
    }
}

public class ApproveWorkOrderInputEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{WorkOrderInputRoot.ApiPath}/{{code}}/approve", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderInputRoot.Tag}")
            .WithName($"Approve {WorkOrderInputRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new ApproveWorkOrderInput(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
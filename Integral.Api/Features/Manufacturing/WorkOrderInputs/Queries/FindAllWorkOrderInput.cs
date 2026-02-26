using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderInputs;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Queries;

public record FindAllWorkOrderInput(int Limit = 100, int Offset = 0, string Search = "")
    : IQuery<FindAllWorkOrderInputResult>
{
}

public record FindAllWorkOrderInputResult(WorkOrderInputHeaderDto[] Data, int Count)
{
}

public class FindAllWorkOrderInputHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindAllWorkOrderInput, FindAllWorkOrderInputResult>
{
    public async Task<FindAllWorkOrderInputResult> Handle(FindAllWorkOrderInput request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.WorkOrderInputs
            .AsNoTracking()
            .OrderByDescending(x => x.Code);

        var res = await query
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        var count = await query.CountAsync(cancellationToken);

        return new FindAllWorkOrderInputResult(res.ToArray(), count);
    }
}

public class FindAllWorkOrderInputEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderInputRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderInputRoot.Tag}")
            .WithName($"Find All {WorkOrderInputRoot.Title}");

        return builder;
    }

    public async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllWorkOrderInput request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
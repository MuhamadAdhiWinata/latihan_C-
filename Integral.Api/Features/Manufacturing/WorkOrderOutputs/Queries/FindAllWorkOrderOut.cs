using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Queries;

public record FindAllWorkOrderOut(int Limit = 100, int Offset = 0, string Search = "") : IQuery<FindAllWorkOrderOutResult>
{
}

public record FindAllWorkOrderOutResult(WorkOrderOutHeaderDto[] Data)
{
}

public class FindAllWorkOrderOutHandler(PrintingDbContext dbContext) : IQueryHandler<FindAllWorkOrderOut, FindAllWorkOrderOutResult>
{
    public async Task<FindAllWorkOrderOutResult> Handle(FindAllWorkOrderOut request, CancellationToken cancellationToken)
    {
        var res = await dbContext.WorkOrderOuts
            .Skip(request.Offset)
            .Take(request.Limit)
            .OrderByDescending(x => x.Code)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);
     
        return new FindAllWorkOrderOutResult(res.ToArray());
    }
}

public class FindAllWorkOrderOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderOutRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderOutRoot.Tag}")
            .WithName($"Find All {WorkOrderOutRoot.Title}");
        
        return builder;
    }

    public async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllWorkOrderOut request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}

using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Dtos;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Queries;

public record FindByCodeWorkOrderOut(string Code) : IQuery<FindByCodeWorkOrderOutResult>
{
}

public record FindByCodeWorkOrderOutResult(WorkOrderOutHeaderDto Header, WorkOrderOutLineDto[] Items)
{
}

public class FindByCodeWorkOrderOutHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindByCodeWorkOrderOut, FindByCodeWorkOrderOutResult>
{
    public async Task<FindByCodeWorkOrderOutResult> Handle(FindByCodeWorkOrderOut request,
        CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderOuts
            .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

        var lines = await dbContext.WorkOrderOutItems
            .Include(x => x.ItemNavigation)
            .Where(x => x.WorkOrderOutId == entry.Id)
            .ToListAsync(cancellationToken);

        entry.Items.Clear();
        foreach (var line in lines)
        {
            entry.Items.Add(line);
        }

        if (entry == null)
            throw new WorkOrderOutNotFoundException(request.Code);

        var itemCodes = entry.Items.Select(i => i.ItemCode).ToArray();

        var masterItems = await dbContext.Items
            .Where(x => itemCodes.AsEnumerable().Contains(x.Code))
            .ToDictionaryAsync(x => x.Code, cancellationToken);

        var header = entry.ToDto();

        var items = entry.Items
            .Select(x =>
            {
                masterItems.TryGetValue(x.ItemCode, out var master);
                return x.ToDto();
            })
            .ToArray();

        return new FindByCodeWorkOrderOutResult(header, items);
    }
}

public class FindByCodeWorkOrderOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderOutRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderOutRoot.Tag}")
            .WithName($"Find By Code {WorkOrderOutRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeWorkOrderOut(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
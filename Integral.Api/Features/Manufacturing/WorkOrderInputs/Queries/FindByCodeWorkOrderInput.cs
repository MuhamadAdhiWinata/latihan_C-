using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Queries;

public record FindByCodeWorkOrderInput(string Code) : IQuery<FindByCodeWorkOrderInputResult>
{
}

public record FindByCodeWorkOrderInputResult(WorkOrderInputHeaderDto Header, WorkOrderInputLineDto[] Items)
{
}

public class FindByCodeWorkOrderInputHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindByCodeWorkOrderInput, FindByCodeWorkOrderInputResult>
{
    public async Task<FindByCodeWorkOrderInputResult> Handle(FindByCodeWorkOrderInput request,
        CancellationToken cancellationToken)
    {
        var entry = await dbContext.WorkOrderInputs
            .Include(x => x.Items)
            .ThenInclude(x => x.ItemNavigation)
            .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);
        
        if (entry == null)
            throw new DomainRuleException(request.Code);

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

        return new FindByCodeWorkOrderInputResult(header, items);
    }
}

public class FindByCodeWorkOrderInputEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderInputRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{WorkOrderInputRoot.Tag}")
            .WithName($"Find By Code {WorkOrderInputRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeWorkOrderInput(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Dtos;
using Integral.Api.Features.Inventories.InventoryIns.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryIns.Features;

public record FindByRefInventoryInResult(InventoryInHeaderDto[] Data);

public record FindByRefInventoryIn(string Code) : IQuery<FindByRefInventoryInResult>;

public class FindByRefInventoryInHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindByRefInventoryIn, FindByRefInventoryInResult>
{
    public async Task<FindByRefInventoryInResult> Handle(FindByRefInventoryIn request,
        CancellationToken cancellationToken)
    {
        var results = await dbContext.F303s
            .AsNoTracking()
            .OrderByDescending(f => f.Iinno)
            .Include(f => f.F304s).ThenInclude(f => f.ItemCodeNavigation)
            .AsSplitQuery()
            .Where(f => f.RefNo == request.Code)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        if (results == null) throw new InventoryInNotFoundException(request.Code);

        return new FindByRefInventoryInResult(results.ToArray());
    }
}

public class FindByRefInventoryInEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/inventory/in/spk/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Inventory In")
            .WithName("Find by Spk Inventory In");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByRefInventoryIn(code);
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
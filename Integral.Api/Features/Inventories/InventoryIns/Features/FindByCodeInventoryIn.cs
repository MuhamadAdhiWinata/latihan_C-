using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Dtos;
using Integral.Api.Features.Inventories.InventoryIns.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryIns.Features;

public record FindByCodeInventoryInResult(InventoryInDtoNew Data);

public record FindByCodeInventoryIn(string Code) : IQuery<FindByCodeInventoryInResult>;

public class FindByCodeInventoryInHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindByCodeInventoryIn, FindByCodeInventoryInResult>
{
    public async Task<FindByCodeInventoryInResult> Handle(FindByCodeInventoryIn request,
        CancellationToken cancellationToken)
    {
        var results = await dbContext.F303s
            .Include(f => f.F304s).ThenInclude(f => f.ItemCodeNavigation)
            .Where(f => f.Iinno == request.Code)
            .Select(x => new InventoryInDtoNew(
                x.ToDto(),
                x.F304s.Select(y => y.ToDto()).ToArray()
                ))
            .FirstOrDefaultAsync(cancellationToken);

        if (results == null) throw new InventoryInNotFoundException(request.Code);

        return new FindByCodeInventoryInResult(results);
    }
}

public class FindByCodeInventoryInEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/inventory/in/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Inventory In")
            .WithName("Find by Code Inventory In");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeInventoryIn(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
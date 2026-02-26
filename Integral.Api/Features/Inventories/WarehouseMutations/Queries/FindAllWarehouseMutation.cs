using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using Integral.Api.Features.Inventories.WarehouseMutations.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.WarehouseMutations.Queries;

public record FindAllWarehouseMutationResult(WarehouseMutationHeaderDto[] Data, int Count);

public record FindAllWarehouseMutation(int Limit = 100, int Offset = 0, string Search = "")
    : IQuery<FindAllWarehouseMutationResult>;

public class FindAllWarehouseMutationHandler(PrintingDbContext printingDb)
    : IQueryHandler<FindAllWarehouseMutation, FindAllWarehouseMutationResult>
{
    public async Task<FindAllWarehouseMutationResult> Handle(FindAllWarehouseMutation request,
        CancellationToken cancellationToken)
    {
        var query = printingDb.F309s
            .OrderByDescending(f => f.Whmno);

        var results = await query
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new FindAllWarehouseMutationResult(results.ToArray(), await query.CountAsync(cancellationToken));
    }
}

public class FindAllWarehouseMutationEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/inventory/warehouse-mutation", Handle)
            .RequireAuthorization()
            .WithTags("Warehouse Mutation")
            .WithName("Find All Warehouse Mutation");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllWarehouseMutation request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
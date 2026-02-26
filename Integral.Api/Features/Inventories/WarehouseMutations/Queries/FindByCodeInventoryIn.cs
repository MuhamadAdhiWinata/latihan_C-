using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using Integral.Api.Features.Inventories.WarehouseMutations.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.WarehouseMutations.Queries;

public record FindByCodeWarehouseMutationResult(object Data);

public record FindByCodeWarehouseMutation(string Code) : IQuery<FindByCodeWarehouseMutationResult>;

public class FindByCodeWarehouseMutationHandler(PrintingDbContext dbContext)
    : IQueryHandler<FindByCodeWarehouseMutation, FindByCodeWarehouseMutationResult>
{
    public async Task<FindByCodeWarehouseMutationResult> Handle(FindByCodeWarehouseMutation request,
        CancellationToken cancellationToken)
    {
        var header = await dbContext.F309s
            .Where(f => f.Whmno == request.Code)
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        var details = await dbContext.F310s
            .Where(f => f.Whmno == request.Code)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new FindByCodeWarehouseMutationResult(new { header, lines = details });
    }
}

public class FindByCodeWarehouseMutationEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/inventory/warehouse-mutation/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Warehouse Mutation")
            .WithName("Find by Code Warehouse Mutation");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeWarehouseMutation(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
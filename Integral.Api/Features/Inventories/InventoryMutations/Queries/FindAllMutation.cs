using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using Integral.Api.Features.Inventories.InventoryMutations.Dto;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Queries;

public record FindAllMutationTransferResult(int Count, MutationHeaderDto[] Data);

public record FindAllMutation(int Limit = 100, int Offset = 0, string Search = "")
    : IQuery<FindAllMutationTransferResult>;

public class FindAllMutationTransferHandler(PrintingDbContext printingDb)
    : IQueryHandler<FindAllMutation, FindAllMutationTransferResult>
{
    public async Task<FindAllMutationTransferResult> Handle(FindAllMutation request,
        CancellationToken cancellationToken)
    {
        var query = printingDb.MutationEntries
            .OrderByDescending(x => x.TransactionCode)
            .Where(x => x.TransactionCode.Contains(request.Search));


        var data = await query
            // .Select(x => x.ToDto())
            .ToArrayAsync(cancellationToken);

        List<MutationHeaderDto> entries = [];
        foreach (var d in data)
        {
            var author = await printingDb.Users.FirstOrDefaultAsync(x => x.Code == d.CreatedBy, cancellationToken);
            entries.Add(d.ToDto(author.Name));
        }

        return new FindAllMutationTransferResult(await query.CountAsync(cancellationToken), entries.ToArray());
    }
}

public class FindAllMutationTransferController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        // builder.MapGet($"{InventoryRoot.BaseApiPath}/merger/deliveries", Handle)
        //     .RequireAuthorization()
        //     .WithTags(DeprecationRoot.DeprecatedTag)
        //     .WithName("Find All Merger Delivery");

        builder.MapGet($"{InventoryMutationRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Find All {InventoryMutationRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllMutation request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
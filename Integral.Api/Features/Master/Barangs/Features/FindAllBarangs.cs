using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Barangs.Features;

public record BarangDto(long Id, string KodeBarang, string NamaBarang);

public record FindAllBarangsResult(int Count, BarangDto[] Data);

public record FindAllBarangs(int Limit = 100, int Offset = 0, string Search = "") : IQuery<FindAllBarangsResult>;

public class FindAllBarangsHandler(PublishingDbContext publishingDb)
    : IQueryHandler<FindAllBarangs, FindAllBarangsResult>
{
    public async Task<FindAllBarangsResult> Handle(FindAllBarangs request, CancellationToken cancellationToken)
    {
        var query = publishingDb.Barangs
            .AsNoTracking()
            .Where(o =>
                o.NamaBarang.Contains(request.Search) ||
                o.KodeBarang.Contains(request.Search));

        var res = await query
            .Select(x => new BarangDto(
                (long)x.Id,
                x.KodeBarang,
                x.NamaBarang
            ))
            .ToListAsync(cancellationToken);

        return new FindAllBarangsResult(query.Count(), res.ToArray());
    }
}

public class FindAllBarangsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryRoot.BaseApiPath}/barangs", Get)
            .RequireAuthorization()
            .WithTags("Barangs")
            .WithName("Find All Barangs");

        return builder;
    }

    public async Task<IResult> Get(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllBarangs request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
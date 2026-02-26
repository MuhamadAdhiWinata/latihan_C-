using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesOrders.Dtos;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesOrders.Queries;

public record FindAllOrder(int Limit = 100, int Offset = 0, string Search = "") : IQuery<FindAllOrderResult>;

public record FindAllOrderResult(OrderHeaderDto[] Data);

public class FindAllOrderHandler(PrintingDbContext dbContext) : IQueryHandler<FindAllOrder, FindAllOrderResult>
{
    public async Task<FindAllOrderResult> Handle(FindAllOrder request, CancellationToken cancellationToken)
    {
        var res = await dbContext.F603s
            .AsNoTracking()
            .OrderByDescending(o => o.Sodno)
            .AsSplitQuery()
            .Skip(request.Offset)
            .Take(request.Limit)
            .Select(o => o.ToDto())
            .ToListAsync(cancellationToken);
        
        return new FindAllOrderResult(res.ToArray());
    }
}

public class FindAllOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{SalesOrderRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .RequirePermission("sales.order.read")
            .WithTags($"{SalesOrderRoot.Title}")
            .WithName($"Find All {SalesOrderRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllOrder request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
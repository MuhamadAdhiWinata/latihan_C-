using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesOrders.Dtos;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesOrders.Queries;

public record FindByCodeOrder(string Code) : IQuery<FindByCodeOrderResult>;

public record FindByCodeOrderResult(OrderDto Data);

public class FindByCodeOrderHandler(PrintingDbContext dbContext) : IQueryHandler<FindByCodeOrder, FindByCodeOrderResult>
{
    public async Task<FindByCodeOrderResult> Handle(FindByCodeOrder request, CancellationToken cancellationToken)
    {
        var order = await dbContext.F603s
            .AsNoTracking()
            .OrderByDescending(o => o.Sodno)
            .Include(o => o.F604s)
            .FirstOrDefaultAsync(o => o.Sodno == request.Code, cancellationToken);

        if (order is null) 
            throw new AppException($"Order {request.Code} not found");
        
        return new FindByCodeOrderResult(order.ToDto(order.F604s.ToArray()));
    }
}

public class FindByCodeWorkOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{SalesOrderRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .RequirePermission("sales.order.read")
            .WithTags($"{SalesOrderRoot.Title}")
            .WithName($"Find By Code {SalesOrderRoot.Title}");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeOrder(code);
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}

using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesDeliveries.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesDeliveries.Queries;

public record GetAllSalesDeliveryResult(int Count, DeliveryHeaderDto[] Data);

public record GetAllSalesDelivery(
    int Limit = 100,
    int Offset = 0,
    string Search = "",
    bool? IsApproved =  null
    
) : IQuery<GetAllSalesDeliveryResult>;

public class GetAllSalesDeliveryHandler(PrintingDbContext printingDb) : IQueryHandler<GetAllSalesDelivery,GetAllSalesDeliveryResult>
{
    public async Task<GetAllSalesDeliveryResult> Handle(GetAllSalesDelivery request, CancellationToken cancellationToken)
    {
        var baseQuery = printingDb.SalesDeliveries
            .AsNoTracking()
            .OrderBy(i => i.Dodno)
            .Where(o =>
                o.Dodno.Contains(request.Search)
            );
        
        if (request.IsApproved.HasValue)
        {
            baseQuery = baseQuery.Where(o => o.Approved == request.IsApproved.Value);
        }
        
        if (baseQuery == null) throw new AppException("Sales Delivery is null");
        
        var count = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Select(o => o.ToDto())
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new GetAllSalesDeliveryResult(count, items.ToArray());
    }
}
public class GetALlSalesDeliveryController : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] GetAllSalesDelivery request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    } 
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{SalesDeliveryRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{SalesDeliveryRoot.Title}")
            .WithName($"Find All {SalesDeliveryRoot.Title}");
        
        return builder;
    }

}
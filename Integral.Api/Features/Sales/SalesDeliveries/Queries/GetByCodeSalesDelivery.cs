using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesDeliveries.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesDeliveries.Queries;


public record GetByCodeSalesDeliveryResult(DeliveryDto Data);

public record GetByCodeSalesDelivery(string Code) : IQuery<GetByCodeSalesDeliveryResult>;

public class GetByCodeSalesDeliveryHandler(PrintingDbContext printingDb) : IQueryHandler<GetByCodeSalesDelivery, GetByCodeSalesDeliveryResult>
{
    public async Task<GetByCodeSalesDeliveryResult> Handle(GetByCodeSalesDelivery request, CancellationToken cancellationToken)
    {
        var delivery = await printingDb.SalesDeliveries
            .Include(u=>u.F606s)
            .Where(u=>u.Dodno == request.Code)
            .Select(x => new DeliveryDto(
                x.ToDto(), 
                x.F606s.Select(y => y.ToDto()).ToArray())
            ).FirstOrDefaultAsync(cancellationToken);
        
        if (delivery is null) throw new AppException($"Sales Delivery dengan kode '{request.Code}' tidak ditemukan.");
        
        return new GetByCodeSalesDeliveryResult(delivery);
    }
}
public class  GetByCodeSalesDeliveryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{SalesDeliveryRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{SalesDeliveryRoot.Title}")
            .WithName($"Find By Code {SalesDeliveryRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    { 
        var request = new GetByCodeSalesDelivery(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
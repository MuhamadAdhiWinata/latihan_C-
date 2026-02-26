using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesDeliveries.Commands;

public record ApproveDelivery(string Code) : ICommand<ApproveDeliveryResult>;

public record ApproveDeliveryResult(string Result);

public class ApproveDeliveryHandler(PrintingDbContext dbContext) : ICommandHandler<ApproveDelivery, ApproveDeliveryResult>
{
    public async Task<ApproveDeliveryResult> Handle(ApproveDelivery request, CancellationToken cancellationToken)
    {
        var delivery = await dbContext.SalesDeliveries.Where(x => x.Dodno == request.Code).FirstOrDefaultAsync(cancellationToken);
        if (delivery == null) throw new Exception();
        delivery.Approve();
        return new ApproveDeliveryResult("Success");
    }
}
public class ApproveSalesDeliveryController : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] ApproveDelivery request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{SalesDeliveryRoot.ApiPath}/{{code}}/approve", Handle)
            .RequireAuthorization()
            .WithTags($"{SalesDeliveryRoot.Title}")
            .WithName($"Approve {SalesDeliveryRoot.Title}");
        
        return builder;
    }

}
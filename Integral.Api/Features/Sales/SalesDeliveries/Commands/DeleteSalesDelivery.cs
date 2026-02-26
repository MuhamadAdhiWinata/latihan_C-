using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesDeliveries.Commands;

public record DeleteSalesDeliveryResult();
public record DeleteSalesDelivery(string Code) : ICommand<DeleteSalesDeliveryResult>;


public class DeleteSalesDeliveryHandler(PrintingDbContext dbContext,CurrentUserProvider currentUser) : ICommandHandler<DeleteSalesDelivery, DeleteSalesDeliveryResult>
{
    public async Task<DeleteSalesDeliveryResult> Handle(DeleteSalesDelivery request, CancellationToken cancellationToken)
    {
        var salesDelivery = await dbContext.SalesDeliveries
            .Include(x => x.F606s)
            .FirstOrDefaultAsync(u=> u.Dodno == request.Code, cancellationToken);
        
        if (salesDelivery == null) throw new DomainRuleException("Delivery not found");
        
        salesDelivery.Delete();
        dbContext.SalesDeliveries.Remove(salesDelivery);
        
        return new DeleteSalesDeliveryResult();
    }
}
public class DeleteSalesDeliveryController : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromRoute] string code)
    {
        var command = new DeleteSalesDelivery(code);
        
        var res = await mediator.Send(command);
        return Results.Empty;
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{SalesDeliveryRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{SalesDeliveryRoot.Title}")
            .WithName($"Delete {SalesDeliveryRoot.Title}");
        return builder;
    }
}
using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryOuts.Commands;

public record DeleteInventoryOutResult();
public record DeleteInventoryOut(string Code) : ICommand<DeleteInventoryOutResult>;

public class DeleteInventoryOutHandler(PrintingDbContext dbContext,CurrentUserProvider currentUser) : ICommandHandler<DeleteInventoryOut, DeleteInventoryOutResult>
{
    public async Task<DeleteInventoryOutResult> Handle(DeleteInventoryOut request, CancellationToken cancellationToken)
    {
        var inventoryOut = await dbContext.InventoryOuts.Include(u => u.F306s)
            .FirstOrDefaultAsync(u => u.Iotno == request.Code, cancellationToken); 
        if (inventoryOut == null) throw new DomainRuleException("Inventory Out not found");
        
        inventoryOut.Delete();
        
        dbContext.InventoryOuts.Remove(inventoryOut);
        
        return new DeleteInventoryOutResult();
    }
}
public class DeleteInventoryOutController : IMinimalEndpoint
{
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromRoute] string code)
    {
        var command = new DeleteInventoryOut(code);
        
        var res = await mediator.Send(command);
        return Results.Empty;
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{InventoryOutRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{InventoryOutRoot.Title}")
            .WithName($"Delete {InventoryOutRoot.Title}");
        
        return builder;
    }
}
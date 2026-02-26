using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Features;

public record UpdateMutation(
    string Code, DateTime TransactionDate, string WarehouseCode,
    string RefNo = "", string Description = "", 
    MutationLineRequestDto[] Items = null!
) : ICommand<UpdateMutationResult>;

public record UpdateMutationResult
{
}

public class UpdateMutationHandler(PrintingDbContext dbContext, CurrentUserProvider currentUser) : ICommandHandler<UpdateMutation, UpdateMutationResult>
{
    public async Task<UpdateMutationResult> Handle(UpdateMutation request, CancellationToken cancellationToken)
    {
        var author = currentUser.GetUsername();
        
        var entry = await dbContext.MutationEntries
            .Include(x => x.MutationItems)
            .Where(x => x.TransactionCode == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (entry == null)
            throw new AppException("Item not found");
        
        var lines = new List<MutationItem>();
        foreach (var item in request.Items)
        {
            var master = await dbContext.Items.FirstOrDefaultAsync(x => x.Code == item.ItemCode, cancellationToken)
                         ?? throw new AppException($"Item {item.ItemCode} not found.");
            
            lines.Add(new MutationItem
            {
                ItemCode = item.ItemCode,
                ItemName = master.Name,
                Quantity = item.Quantity,
                Price = 0,
                SellingPrice = item.SellingPrice,
            });
        }
        
        entry.Update(request.TransactionDate, request.WarehouseCode, request.RefNo, request.Description, author, lines.ToArray());
        
        return new UpdateMutationResult();
    }
}

public class UpdateMutationEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{InventoryMutationRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Update {InventoryMutationRoot.Title}");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] UpdateMutationEndpointBody request,
        string code)
    {
        var command = new UpdateMutation (code, request.TransactionDate, request.WarehouseCode, request.RefNo, request.Description, request.Items);
        var res = await mediator.Send(command);
        
        return Results.Ok(res);
    }

    private record UpdateMutationEndpointBody(
        DateTime TransactionDate,
        string WarehouseCode,
        string RefNo = "",
        string Description = "",
        MutationLineRequestDto[] Items = null!);
}
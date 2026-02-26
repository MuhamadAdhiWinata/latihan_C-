using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryMutations.Features;

public record MutationLineRequestDto(string ItemCode, int Quantity, decimal SellingPrice);

public record CreateMutation(DateTime TransactionDate, string WarehouseCode, string RefNo = "", string Description = "", MutationLineRequestDto[] Items = null!) : ICommand<CreateMutationResult>;

public record CreateMutationResult(string Code);

public class CreateMutationTransferHandler(PrintingDbContext dbContext, CurrentUserProvider currentUserProvider)
    : ICommandHandler<CreateMutation, CreateMutationResult>
{
    public async Task<CreateMutationResult> Handle(CreateMutation request, CancellationToken cancellationToken)
    {
        var codegen = new CodeGenerator(dbContext);
        var code = await codegen.GenerateAsync<MutationEntry>("DOMT", nameof(MutationEntry.TransactionCode));
        var author = currentUserProvider.GetUsername();
        
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
        
        var entry = MutationEntry.Create(
            code,  request.TransactionDate, request.WarehouseCode, request.RefNo, 
            request.Description, author,
            lines.ToArray());
        
        await dbContext.MutationEntries.AddAsync(entry, cancellationToken);
        return new  CreateMutationResult(entry.TransactionCode);
    }
}

public class CreateMutationTransferController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{InventoryMutationRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags(InventoryMutationRoot.Tags)
            .WithName($"Create {InventoryMutationRoot.Title}");
        
        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateMutation request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
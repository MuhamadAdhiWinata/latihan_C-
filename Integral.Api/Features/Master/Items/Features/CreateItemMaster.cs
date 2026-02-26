using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Items.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Items.Features;

public record CreateItemMaster(
    string ItemCode,
    string ItemName,
    string UnitCode,
    decimal Price,
    string? Sku,
    string ItemTypeCode,
    short ActiveStatus,
    string MerkCode) : ICommand<CreateItemResult>;

public record CreateItemResult(string Code)
{
}

public class CreateItemHandler(PrintingDbContext printingDb, CurrentUserProvider currentUser)
    : ICommandHandler<CreateItemMaster, CreateItemResult>
{
    public async Task<CreateItemResult> Handle(CreateItemMaster request, CancellationToken cancellationToken)
    {
        var existingItem = await printingDb.Items.FirstOrDefaultAsync(x => x.Code == request.ItemCode, cancellationToken);
        if (existingItem != null)
            throw new InvalidOperationException($"Item with {request.ItemCode} already exist.");

        var user = currentUser.GetUsername();
        
        var item = Item.Create(request.ItemCode, request.ItemName, request.UnitCode, request.Price, request.Sku ?? "", request.ItemTypeCode);
        item.CreatedBy = user;

        await printingDb.Items.AddAsync(item, cancellationToken);
        return new CreateItemResult(item.Code);
    }
}

public class CreateItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{ItemRoot.BaseApiPath}", Handle)
            .RequireAuthorization()
            .WithTags("Master Items")
            .WithName("Create Item Master");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateItemMaster request)
    {
        return Results.Ok(await mediator.Send(request));
    }
}
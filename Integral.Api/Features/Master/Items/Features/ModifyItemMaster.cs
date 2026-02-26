using Integral.Api.Data.Contexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Items.Features;

public record ModifyItemMaster(
    string ItemCode,
    string ItemName,
    string UnitCode,
    decimal Price,
    string? Sku,
    string ItemTypeCode,
    short ActiveStatus,
    string MerkCode) : ICommand;

public class ModifyItemHandler(PrintingDbContext printingDb) : ICommandHandler<ModifyItemMaster>
{
    public async Task<Unit> Handle(ModifyItemMaster request, CancellationToken cancellationToken)
    {
        var item = await printingDb.Items.FirstOrDefaultAsync(x => x.Code == request.ItemCode, cancellationToken);
        if (item == null)
            throw new InvalidOperationException($"Item {request.ItemCode} not found.");

        item.Modify(request.ItemName, request.UnitCode, request.Price, request.Sku ?? "", request.ItemTypeCode);

        return Unit.Value;
    }
}

public class ModifyItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{ItemRoot.BaseApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags("Master Items")
            .WithName("Modify Item Master");

        return builder;
    }

    public async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] ModifyItemMasterParameter request,
        string code)
    {
        var command = new ModifyItemMaster(code, request.ItemName, request.UnitCode, request.Price, request.Sku,
            request.ItemTypeCode, request.ActiveStatus, request.MerkCode);

        var res = await mediator.Send(command);

        return Results.Ok(res);
    }

    public record ModifyItemMasterParameter(
        string ItemName,
        string UnitCode,
        decimal Price,
        string? Sku,
        string ItemTypeCode,
        short ActiveStatus,
        string MerkCode);
}
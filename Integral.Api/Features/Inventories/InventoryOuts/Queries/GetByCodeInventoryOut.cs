using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryOuts.Queries;

public record GetByCodeInventoryOutResult(InventoryOutDto Data);

public record GetByCodeInventoryOut(string Code) : IQuery<GetByCodeInventoryOutResult>;

public class GetByCodeInventoryOutHandler(PrintingDbContext printingDb)
    : IQueryHandler<GetByCodeInventoryOut, GetByCodeInventoryOutResult>
{
    public async Task<GetByCodeInventoryOutResult> Handle(GetByCodeInventoryOut request,
        CancellationToken cancellationToken)
    {
        var iout = await printingDb.InventoryOuts
            .Include(u => u.F306s)
            .ThenInclude(x => x.ItemNavigation)
            .Where(u => u.Iotno == request.Code)
            .Select(x => new InventoryOutDto(
                x.ToDto(),
                x.F306s.Select(y => y.ToDto()).ToArray())
            ).FirstOrDefaultAsync(cancellationToken);

        if (iout is null) throw new AppException($"Inventory Out dengan kode '{request.Code}' tidak ditemukan.");

        return new GetByCodeInventoryOutResult(iout);
    }
}

public class GetByCodeInventoryOutEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{InventoryOutRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags($"{InventoryOutRoot.Title}")
            .WithName($"Find By Code {InventoryOutRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new GetByCodeInventoryOut(code);
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
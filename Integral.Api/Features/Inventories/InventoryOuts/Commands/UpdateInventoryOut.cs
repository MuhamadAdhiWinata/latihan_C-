using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Dtos;
using Integral.Api.Features.Inventories.InventoryOuts.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryOuts.Commands;
public record UpdateInventoryOutResult(InventoryOutDto Data);

public record UpdateInventoryOut(
    string Code,
    DateTime TransactionDate,
    string RefNo,
    string WarehouseCode,
    string Description,
    List<InventoryOutLineRequestDto> Items
) : ICommand<UpdateInventoryOutResult>;

public class UpdateInventoryOutHandler(PrintingDbContext dbContext, CurrentUserProvider currentUser) : ICommandHandler<UpdateInventoryOut, UpdateInventoryOutResult>
{
    public async Task<UpdateInventoryOutResult> Handle(UpdateInventoryOut request, CancellationToken cancellationToken)
    {
        var user =  currentUser.GetUsername();
          
        var entity = await dbContext.InventoryOuts
            .Include(u => u.F306s)
            .FirstOrDefaultAsync(u=>u.Iotno == request.Code, cancellationToken);
        
        if (entity == null) throw new AppException("Inventory Out Not Found");
        
        
        entity.Update(
            request.TransactionDate,    
            request.RefNo,
            request.WarehouseCode,
            request.Description,
            user,
            request.Items.Select(x =>
            {
                var item = dbContext.Items.FirstOrDefault(y => x.ItemCode == y.Code);
                if (item == null) throw new DomainRuleException($"Item {x.ItemCode} not found");

                return new InventoryOutLine()
                {
                    ItemCode = x.ItemCode,
                    ReasonCode = x.ReasonCode,
                    Quantity = x.Quantity,
                    CreatedBy = user,
                    Description = x.Description,
                    Iotno = request.Code
                };
            }).ToArray());

        return new UpdateInventoryOutResult(new InventoryOutDto(
            entity.ToDto(),
            entity.F306s.Select(x => x.ToDto()).ToArray()
        ));
    }
}

public class UpdateInventoryOutController : IMinimalEndpoint
{
    private record UpdateInventoryOutRequestDto(
        DateTime TransactionDate,
        string RefNo,
        string WarehouseCode,
        string Description,
        List<InventoryOutLineRequestDto> Items);

    
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromRoute] string code, 
        [FromBody] UpdateInventoryOutRequestDto dto 
    )
    {
        var request = new UpdateInventoryOut(code, dto.TransactionDate, dto.RefNo, dto.WarehouseCode, dto.Description, dto.Items);
        
        var res =  await mediator.Send(request);
        return Results.Ok(res);
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{InventoryOutRoot.ApiPath}/{{code}}", Handle) 
            .RequireAuthorization()
            .WithTags($"{InventoryOutRoot.Title}")
            .WithName($"Update {InventoryOutRoot.Title}");
        
        return builder;
    }
}
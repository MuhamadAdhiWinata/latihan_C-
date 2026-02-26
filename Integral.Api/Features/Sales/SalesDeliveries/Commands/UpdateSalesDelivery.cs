using FluentValidation;
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesDeliveries.Dtos;
using Integral.Api.Features.Sales.SalesDeliveries.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesDeliveries.Commands;

public record UpdateSalesDeliveryResult(DeliveryDto Data);

public record UpdateSalesDelivery(
    string Code,
    DateTime TransactionDate,
    string CustomerCode,
    string OrderCode,
    string WarehouseCode,
    string Description,
    List<SalesDeliveryLineRequestDto> Items
) : ICommand<UpdateSalesDeliveryResult>;

public class UpdateSalesDeliveryValidator : AbstractValidator<UpdateSalesDelivery>
{
    public UpdateSalesDeliveryValidator()
    {
        RuleFor(x => x.Code).NotNull();
        RuleFor(x => x.TransactionDate).NotNull();
        RuleFor(x => x.CustomerCode).NotNull();
        RuleFor(x => x.WarehouseCode).NotNull();
        RuleFor(x => x.OrderCode).NotNull();
        RuleFor(x => x.Items).NotEmpty();
    }
}

public class UpdateSalesDeliveryHandler(PrintingDbContext dbContext, CurrentUserProvider currentUser) : ICommandHandler<UpdateSalesDelivery,UpdateSalesDeliveryResult>
{
    public async Task<UpdateSalesDeliveryResult> Handle(UpdateSalesDelivery request, CancellationToken cancellationToken)
    {
        var user = currentUser.GetUsername();
        
        var entity = await dbContext.SalesDeliveries
            .Include(u => u.F606s)
            .FirstOrDefaultAsync(u=>u.Dodno == request.Code, cancellationToken);
        
        if (entity == null) throw new AppException("Sales Delivery Not Found");

        
        var order = await dbContext.F603s
            .Include(x => x.F604s)
            .Where(x => x.Sodno == request.OrderCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null) throw new AppException($"Sales order {request.OrderCode} not found");
        
        entity.Update(
            request.TransactionDate,
            request.CustomerCode,
            request.OrderCode,
            request.WarehouseCode,
            request.Description,
            order.TaxStatus,
            user,
            request.Items.Select(x =>
            {
                var item = order.F604s.FirstOrDefault(y => x.ItemCode == y.ItemCode);
                if (item == null) throw new DomainRuleException($"Item {x.ItemCode} not found");
                
                return new SalesDeliveryLine()
                {
                    ItemCode = x.ItemCode,
                    ItemAlias = x.ItemAlias,
                    Quantity = x.Quantity,
                    Price = item.Price,
                    CreatedBy = user,
                    CreatedDate = DateTime.Now,
                    Description = x.Description
                };
            }).ToArray());

        return new UpdateSalesDeliveryResult(new DeliveryDto(
            entity.ToDto(),
            entity.F606s.Select(x => x.ToDto()).ToArray()
            ));
    }
}
public class UpdateSalesDeliveryController : IMinimalEndpoint
{
    private record UpdateDeliveryRequestDto(
        DateTime TransactionDate,
        string CustomerCode,
        string OrderCode,
        string WarehouseCode,
        string Description,
        List<SalesDeliveryLineRequestDto> Items);

    
    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromRoute] string code, 
        [FromBody] UpdateDeliveryRequestDto dto 
    )
    {
        var request = new UpdateSalesDelivery(code, dto.TransactionDate, dto.CustomerCode, dto.OrderCode, dto.WarehouseCode, dto.Description, dto.Items);
        
        var res =  await mediator.Send(request);
        return Results.Ok(res);
    }
    
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{SalesDeliveryRoot.ApiPath}/{{code}}", Handle) 
            .RequireAuthorization()
            .WithTags($"{SalesDeliveryRoot.Title}")
            .WithName($"Update {SalesDeliveryRoot.Title}");
        
        return builder;
    }
}
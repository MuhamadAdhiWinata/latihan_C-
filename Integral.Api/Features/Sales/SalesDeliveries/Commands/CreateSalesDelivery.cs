using FluentValidation;
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Sales.SalesDeliveries.Dtos;
using Integral.Api.Features.Sales.SalesDeliveries.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Sales.SalesDeliveries.Commands;

public record CreateSalesDeliveryResult(string Code);
public record CreateSalesDelivery(
        DateTime TransactionDate,
        string CustomerCode,
        string OrderCode,
        string WarehouseCode,
        string Description,
        List<SalesDeliveryLineRequestDto> Items
    ):ICommand<CreateSalesDeliveryResult>;

public class CreateSalesDeliveryValidator : AbstractValidator<CreateSalesDelivery>
{
    public CreateSalesDeliveryValidator()
    {
        RuleFor(x => x.TransactionDate).NotNull();
        RuleFor(x => x.CustomerCode).NotNull();
        RuleFor(x => x.WarehouseCode).NotNull();
        RuleFor(x => x.OrderCode).NotNull();
        
        RuleFor(x => x.Items).NotEmpty();
    }
}

public class CreateSalesDeliveryHandler(PrintingDbContext dbContext,CurrentUserProvider currentUser):ICommandHandler<CreateSalesDelivery,CreateSalesDeliveryResult>
{
    public async Task<CreateSalesDeliveryResult> Handle(CreateSalesDelivery request, CancellationToken cancellationToken)
    {
        var generator = new CodeGenerator(dbContext);
        var user = currentUser.GetUsername();
        
        var order = await dbContext.F603s
            .Include(x => x.F604s)
            .Where(x => x.Sodno == request.OrderCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null) throw new DomainRuleException($"Sales order {request.OrderCode} not found");
        var newDeliveryCode = await generator.GenerateAsync<SalesDelivery>("DOD", nameof(SalesDelivery.Dodno));
        
        var delivery = SalesDelivery.Create(
            newDeliveryCode,
            request.TransactionDate,
            request.CustomerCode,
            request.OrderCode,
            request.WarehouseCode,
            request.Description,
            order.TaxStatus,
            user,
            order.TotalTransactionAmount,
            order.Ppnamount,
            order.Ppnpercent,
            request.Items.Select(x =>
            {
                var item = order.F604s.FirstOrDefault(y => x.ItemCode == y.ItemCode);
                if (item == null) throw new DomainRuleException($"Item {x.ItemCode} not found");
                
                return new SalesDeliveryLine()
                {
                    Dodno =newDeliveryCode,
                    ItemCode = x.ItemCode,
                    ItemAlias = x.ItemAlias,
                    Quantity = x.Quantity,
                    Price = item.Price,
                    CreatedBy = user,
                    CreatedDate = DateTime.Now,
                    Description = x.Description,      
                    
                };
            }).ToArray());
        
        await dbContext.SalesDeliveries.AddAsync(delivery, cancellationToken);
        return new CreateSalesDeliveryResult(delivery.Dodno);
    }
}

public class CreateSalesDeliveryController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{SalesDeliveryRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{SalesDeliveryRoot.Title}")
            .WithName($"Create {SalesDeliveryRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateSalesDelivery request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
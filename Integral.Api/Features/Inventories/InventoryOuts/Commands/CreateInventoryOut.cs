using FluentValidation;
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryOuts.Dtos;
using Integral.Api.Features.Inventories.InventoryOuts.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Domain;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryOuts.Commands;


public record CreateInventoryOutResult(string Code);
public record CreateInventoryOut(
    DateTime TransactionDate,
    string RefNo,
    string WarehouseCode,
    string Description,
    List<InventoryOutLineRequestDto> Items
):ICommand<CreateInventoryOutResult>;

public class CreateInventoryOutValidator : AbstractValidator<CreateInventoryOut>
{
    public CreateInventoryOutValidator()
    {
        RuleFor(x => x.TransactionDate).NotNull();
        RuleFor(x => x.RefNo).NotNull();
        RuleFor(x => x.WarehouseCode).NotNull();
        RuleFor(x => x.Items).NotEmpty();
    }
}

public class CreateInventoryOutHandler(PrintingDbContext dbContext,CurrentUserProvider currentUser) : ICommandHandler<CreateInventoryOut, CreateInventoryOutResult>
{
    public async Task<CreateInventoryOutResult> Handle(CreateInventoryOut request, CancellationToken cancellationToken)
    {
        var generator = new CodeGenerator(dbContext);
        var user = currentUser.GetUsername();

        var inventoryOut = InventoryOut.Create(
            await generator.GenerateAsync<InventoryOut>("IOT", nameof(InventoryOut.Iotno)),
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
                };
            }).ToArray());
        
        await dbContext.InventoryOuts.AddAsync(inventoryOut, cancellationToken);
        return new CreateInventoryOutResult(inventoryOut.Iotno);
    }
}

public class CreateInventoryOutController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{InventoryOutRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{InventoryOutRoot.Title}")
            .WithName($"Create {InventoryOutRoot.Title}");

        return builder;
    }

    private async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateInventoryOut request)
    {
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
using FluentValidation;
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryIns.Features;

public record CreateInventoryInResult(string Code);

public abstract record CreateInventoryInLine(
    string ItemCode,
    string UnitCode,
    decimal Quantity,
    decimal Price,
    string Description
);

public record CreateInventoryIn(
    DateTime TransactionDate,
    string WarehouseCode,
    string SpkCode,
    string Description,
    List<CreateInventoryInLine> Details) : ICommand<CreateInventoryInResult>;

public class CreateInventoryInValidator : AbstractValidator<CreateInventoryIn>
{
    public CreateInventoryInValidator()
    {
        RuleFor(x => x.TransactionDate).NotEmpty();
        RuleFor(x => x.WarehouseCode).NotEmpty();
        RuleFor(x => x.SpkCode).NotEmpty();
        RuleFor(x => x.Details)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new CreateInventoryInDetailValidator()));
    }
    
    class CreateInventoryInDetailValidator : AbstractValidator<CreateInventoryInLine>
    {
        public CreateInventoryInDetailValidator()
        {
            RuleFor(x => x.ItemCode).NotEmpty();
            RuleFor(x => x.UnitCode).NotEmpty();
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price cannot be negative.");
        }
    }
}



public class CreateInventoryInHandler(PrintingDbContext dbContext, CurrentUserProvider currentUser, IMediator mediator)
    : ICommandHandler<CreateInventoryIn, CreateInventoryInResult>
{
    public async Task<CreateInventoryInResult> Handle(CreateInventoryIn request, CancellationToken cancellationToken)
    {
        var generator = new CodeGenerator(dbContext);
        var user = currentUser.GetUsername();

        var code = await generator.GenerateAsync<F303>("IIN", nameof(F303.Iinno));
        var entry = F303.Create(
            code,
            request.TransactionDate,
            AppDefaults.BranchCode,
            request.SpkCode,
            AppDefaults.CurrencyCode,
            AppDefaults.ExchangeRate,
            request.WarehouseCode,
            request.Description,
            currentUser.GetUsername(),
            request.Details.Select(detail => new F304()
            {
                BranchCode = AppDefaults.BranchCode,
                Iinno = code,
                ItemCode = detail.ItemCode,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Description = detail.Description,
                CreatedBy = user,
                CreatedDate = DateTime.Now,
                UpdatedBy = "",
                UpdatedDate = DateTime.Now
            }));

        await dbContext.F303s.AddAsync(entry, cancellationToken);

        return new CreateInventoryInResult(entry.Iinno);
    }
}

public class CreateInternalDeliveryController : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{InventoryRoot.BaseApiPath}/inventory/in", Handle)
            .RequireAuthorization()
            .WithTags("Inventory In")
            .WithName("Create Inventory In");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromBody] CreateInventoryIn request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Queries;

public record FindByCodeWorkOrderResult(
    WorkOrderDto Header,
    decimal BopTotal,
    WorkOrderItemDto[] Items,
    WorkOrderMaterialDto[] Materials,
    WorkOrderDetailDto[] Machines,
    WorkOrderDetailDto[] Labors,
    WorkOrderDetailDto[] Finishings,
    WorkOrderDetailDto[] Packagings);

public record FindByCodeWorkOrder(string Code) : IQuery<FindByCodeWorkOrderResult>;

public class FindByCodeWorkOrderHandler(
    PrintingDbContext dbContext,
    IMediator mediator,
    WorkOrderInputQueryService woinQuery)
    : IQueryHandler<FindByCodeWorkOrder, FindByCodeWorkOrderResult>
{
    public async Task<FindByCodeWorkOrderResult> Handle(FindByCodeWorkOrder request,
        CancellationToken cancellationToken)
    {
        var entries = await mediator.Send(new FindProductionHistoryWorkOrder(request.Code), cancellationToken);
        var finishedItems = entries.Data
            .SelectMany(x => x.Lines)
            .GroupBy(d => d.ItemCode)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Quantity));

        var wo = await dbContext.WorkOrders
            .AsNoTracking()
            .OrderByDescending(o => o.Dodno)
            .AsSplitQuery()
            .Include(o => o.Items)
            .Include(o => o.Materials)
            .Include(o => o.Machines)
            .Include(o => o.Labors)
            .Include(o => o.Finishings)
            .Include(o => o.Packagings)
            .FirstOrDefaultAsync(o => o.Dodno == request.Code, cancellationToken);

        if (wo is null)
            throw new InvalidOperationException($"WorkOrder with code '{request.Code}' not found.");

        var header = new WorkOrderDto(
            wo.BranchCode,
            wo.Dodno,
            wo.TransactionDate,
            wo.CustomerCode,
            wo.Sodno,
            wo.Description,
            wo.Prepressinstruction,
            wo.Pressinstruction,
            wo.Finishinginstruction,
            wo.Packaginginstruction,
            wo.Status,
            wo.DeleteStatus,
            wo.Spdescription,
            wo.Approved,
            wo.Approved2,
            wo.Isprepress,
            wo.Ispress,
            wo.Isfinishing,
            wo.Ispackaging
        );

        var items = wo.Items
            .Select(i => new WorkOrderItemDto(
                i.BranchCode,
                i.Dodno,
                i.ItemCode,
                i.ItemAlias,
                i.Quantity,
                Math.Max(0, i.Quantity - finishedItems.GetValueOrDefault(i.ItemCode, 0)),
                i.Price,
                i.Description,
                i.DiscountPercent,
                i.DiscountPercent1,
                i.DiscountPercent2,
                i.DiscountPercent3,
                i.DiscountAmount1,
                i.DiscountAmount2,
                i.DiscountAmount3,
                i.CreatedBy,
                i.CreatedDate,
                i.UpdatedBy,
                i.UpdatedDate,
                i.BonusQuantity,
                i.AlterdBy,
                i.AlterdDate
            ))
            .ToArray();

        var materialInputs = await woinQuery.FindByWorkOrder(wo.Dodno, cancellationToken);

        var materials = wo.Materials.Select(x => new WorkOrderMaterialDto(
            x.BranchCode,
            x.Spk,
            x.ItemCode,
            x.ItemAlias,
            x.Quantity,
            materialInputs.Sum(mi =>
                mi.Items
                    .Where(i => i.ItemCode == x.ItemCode)
                    .Sum(i => i.Quantity)
            ),
            x.Price,
            x.Description,
            x.CreatedBy,
            x.CreatedDate,
            x.UpdatedBy,
            x.UpdatedDate
        )).ToArray();


        WorkOrderDetailDto[] MapDetails(IEnumerable<dynamic> list)
        {
            return list.Select(m => new WorkOrderDetailDto(
                m.BranchCode,
                m.Spk,
                m.ItemCode,
                m.ItemAlias,
                m.Quantity,
                m.Price,
                m.Description,
                m.CreatedBy,
                m.CreatedDate,
                m.UpdatedBy,
                m.UpdatedDate
            )).ToArray();
        }

        var machines = MapDetails(wo.Machines);
        var labors = MapDetails(wo.Labors);
        var finishings = MapDetails(wo.Finishings);
        var packagings = MapDetails(wo.Packagings);

        var bopTotal =
            materials.Sum(m => m.Quantity * m.Price) +
            machines.Sum(m => m.Quantity * m.Price) +
            labors.Sum(m => m.Quantity * m.Price) +
            finishings.Sum(m => m.Quantity * m.Price) +
            packagings.Sum(m => m.Quantity * m.Price);

        return new FindByCodeWorkOrderResult(
            header,
            bopTotal,
            items,
            materials,
            machines,
            labors,
            finishings,
            packagings
        );
    }
}

public class FindByCodeWorkOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderRoot.ApiPath}/{{code}}", Handle)
            .RequireAuthorization()
            .WithTags(WorkOrderRoot.Tag)
            .WithName($"Find By Code {WorkOrderRoot.Title}");


        return builder;
    }

    public async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        string code)
    {
        var request = new FindByCodeWorkOrder(code);
        var res = await mediator.Send(request);
        return Results.Ok(res);
    }
}
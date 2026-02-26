using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrders.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Queries;

public record FindAllWorkOrderResult(int Count, WorkOrderDto[] Data);

public record FindAllWorkOrder(int Limit = 100, int Offset = 0, string Search = "", bool? IsFinished = null) : IQuery<FindAllWorkOrderResult>;

public class FindAllWorkOrderHandler(PrintingDbContext dbContext, IMediator mediator, CurrentUserProvider currentUser)
    : IQueryHandler<FindAllWorkOrder, FindAllWorkOrderResult>
{
    public async Task<FindAllWorkOrderResult> Handle(FindAllWorkOrder query, CancellationToken cancellationToken)
    {
        
        var baseQuery =  dbContext.WorkOrders
            .OrderByDescending(o => o.Dodno)
            .Where(x => x.Dodno.Contains(query.Search) || x.Description.Contains(query.Search))
            .Where(x => query.IsFinished == null || (query.IsFinished.Value
                ? x.Status == WorkOrderStatus.Finished
                : x.Status != WorkOrderStatus.Finished));

        var data = await baseQuery
            .Skip(query.Offset)
            .Take(query.Limit)
            .Select(wo => new WorkOrderDto(
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
                wo.Ispackaging))
            .ToListAsync(cancellationToken);

        var count = await baseQuery.CountAsync(cancellationToken);

        return new FindAllWorkOrderResult(count, data.ToArray());
    }
}

public class FindAllWorkOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{WorkOrderRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags(WorkOrderRoot.Tag)
            .WithName($"Find All {WorkOrderRoot.Title}");
        
        return builder;
    }

    public async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllWorkOrder request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}
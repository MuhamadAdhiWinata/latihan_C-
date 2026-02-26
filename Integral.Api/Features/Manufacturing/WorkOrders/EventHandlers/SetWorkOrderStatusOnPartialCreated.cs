using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction;

namespace Integral.Api.Features.Manufacturing.WorkOrders.EventHandlers;

public class SetWorkOrderStatusOnPartialCreated(PrintingDbContext dbContext)
    : INotificationHandler<WorkOrderOutApproved>
{
    public async Task Handle(WorkOrderOutApproved notification, CancellationToken cancellationToken)
    {
        var wo = await dbContext.WorkOrders
            .Where(x => x.Dodno == notification.WorkOrderCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (wo == null) throw new AppException("Work Order Not Found");

        wo.Partial();
    }
}
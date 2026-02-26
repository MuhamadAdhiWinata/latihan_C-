using Integral.Api.Data.Contexts;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Queries;

public class WorkOrderInputQueryService(PrintingDbContext dbContext)
{
    public async Task<WorkOrderInput[]> FindByWorkOrder(string workOrderCode,
        CancellationToken cancellationToken = default)
    {
        var res = await dbContext.WorkOrderInputs
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.WorkOrderCode == workOrderCode)
            .ToArrayAsync(cancellationToken);

        return res;
    }
}
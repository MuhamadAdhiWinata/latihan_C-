using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Models;
using Integral.Api.Features.Inventories.WarehouseMutations.Entities;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Events;
using MediatR;
using SharedKernel;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.WarehouseMutations.EventHandlers;

public class CreateOnWorkOrderInputApproved(PrintingDbContext dbContext, CurrentUserProvider currentUser)
    : INotificationHandler<WorkOrderInputApproved>
{
    public async Task Handle(WorkOrderInputApproved request, CancellationToken cancellationToken)
    {
        var generator = new CodeGenerator(dbContext);
        var user = currentUser.GetUsername();

        var code = await generator.GenerateAsync<F309>("WHM", nameof(F309.Whmno));
        var entry = F309.Create(
            code,
            request.TransactionDate,
            "gudang bahan baku",
            "GUDANG PRODUKSI",
            "",
            currentUser.GetUsername(),
            request.Items.Select(detail => new F310()
            {
                BranchCode = AppDefaults.BranchCode,
                ItemCode = detail.ItemCode,
                Quantity = detail.Quantity,
                Price = detail.Price,
                Description = detail.Description,
                CreatedBy = user,
                CreatedDate = DateTime.Now,
                UpdatedBy = "",
                UpdatedDate = DateTime.Now
            }),
            request.WorkOrderCode);

        await dbContext.AddAsync(entry, cancellationToken);
    }
}
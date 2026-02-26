using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryIns.Models;
using Integral.Api.Features.Manufacturing.WorkOrders.Events;
using MediatR;
using SharedKernel;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Inventories.InventoryIns.EventHandler;

public class CreateOnWorkOrderOutApproved(PrintingDbContext dbContext, CurrentUserProvider currentUser) : INotificationHandler<WorkOrderOutApproved>
{
    public async Task Handle(WorkOrderOutApproved request, CancellationToken cancellationToken)
    { 
        var generator = new CodeGenerator(dbContext);
        var user = currentUser.GetUsername();

        var code = await generator.GenerateAsync<F303>("IIN", nameof(F303.Iinno));
        var entry = F303.Create(
            code,
            request.TransactionDate,
            AppDefaults.BranchCode,
            request.WorkOrderCode,
            AppDefaults.CurrencyCode,
            AppDefaults.ExchangeRate,
            "gudang bahan jadi",
            request.Description,
            currentUser.GetUsername(),
            request.Items.Select(detail => new F304()
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
            }));

        await dbContext.F303s.AddAsync(entry, cancellationToken);
    }
}
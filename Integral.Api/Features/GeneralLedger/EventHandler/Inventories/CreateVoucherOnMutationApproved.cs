using Integral.Api.Data.Contexts;
using Integral.Api.Features.GeneralLedger.Model;
using Integral.Api.Features.Inventories.InventoryMutations.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.GeneralLedger.EventHandler.Inventories;

public class CreateVoucherOnMutationApproved(PrintingDbContext dbContext, CurrentUserProvider currentUser) : INotificationHandler<MutationTransferApproved>
{
    public async Task Handle(MutationTransferApproved notification, CancellationToken cancellationToken)
    {
        const string type = "PJR";
        const string akunPerantara = "10.05.01";
        const string gudang = "gudang bahan jadi";
        
        var codegen = new CodeGenerator(dbContext);
        var user = currentUser.GetUsername();
        var code = await codegen.GenerateAsync<Voucher>(type, nameof(Voucher.VoucherNo));

        var lines = new List<VoucherLine>();
        
        foreach (var detail in notification.Details)
        {
            var item = await dbContext.Items
                .Include(x => x.ItemType)
                .FirstOrDefaultAsync(x => x.Code == detail.ItemCode, cancellationToken);

            if (item == null) throw new AppException("Item not found");
            
            var warehouseAccount = await dbContext.F015Bs
                    .Where(x => x.Whcode == gudang)
                    .Where(x => x.Code == item.ItemTypeCode)
                    .FirstOrDefaultAsync(cancellationToken);
            
            if (warehouseAccount == null) throw new AppException("Warehouse account not found");
            
            var amount = detail.Quantity * detail.Price;
            
            lines.Add(VoucherLine.Create( code, warehouseAccount.AccountCode, 0, amount, $"{detail.ItemName}", user ));
            lines.Add(VoucherLine.Create( code, akunPerantara, amount, 0,  $"Dana Perantara {detail.ItemName}", user ));
        }
        
        var entry = Voucher.Create(
            await codegen.GenerateAsync<Voucher>(type, nameof(Voucher.VoucherNo)),
            type,
            notification.Code,
            user,
            lines.ToArray()
        );

        await dbContext.Journals.AddAsync(entry, cancellationToken);
    }
}
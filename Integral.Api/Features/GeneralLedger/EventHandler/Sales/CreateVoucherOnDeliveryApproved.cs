// using Integral.Api.Data.Contexts;
// using Integral.Api.Features.GeneralLedger.Model;
// using Integral.Api.Features.SalesDeliveries.Entities;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using SharedKernel;
// using SharedKernel.Abstraction.Web;
//
// namespace Integral.Api.Features.GeneralLedger.EventHandler.Sales;
//
// public class CreateVoucherOnDeliveryApproved(PrintingDbContext dbContext, CurrentUserProvider currentUser) : INotificationHandler<DeliveryApproved>
// {
//     public async Task Handle(DeliveryApproved notification, CancellationToken cancellationToken)
//     { 
//         var codegen = new CodeGenerator(dbContext);
//         var delivery = await dbContext.SalesDeliveries
//             .Include(x => x.F606s)
//             .Where(x => x.Dodno == notification.Code)
//             .FirstOrDefaultAsync(cancellationToken);
//         
//         if (delivery == null) throw new Exception("Invoice not found");
//
//         const string piutangDagang = "10.01.03.01";
//         const string hutangPajak = "20.01.01.03.01";
//         const string prefix = "SJR";
//
//         var code = await codegen.GenerateAsync<Voucher>(prefix, nameof(Voucher.VoucherNo));
//
//         var lines = new List<VoucherLine>();
//         // {
//         //     VoucherLine.Create(code, piutangDagang, delivery.GrandTotal, 0, ""),
//         //     VoucherLine.Create(code, delivery.BankAccountCode, 0, delivery.GrandTotal - delivery.Ppnamount, "")
//         // };
//
//         foreach (var deliveryLine in delivery.F606s)
//         {
//             var item = await dbContext.Items
//                 .Include(x => x.ItemType)
//                 .FirstOrDefaultAsync(x => x.Code == deliveryLine.ItemCode, cancellationToken);
//
//             if (item == null) throw new Exception();
//
//             var accountWarehouseMap =
//                 await dbContext.F015Bs.FirstOrDefaultAsync(x => x.Code == item.ItemTypeCode && x.Whcode == delivery.WarehouseCode, cancellationToken);
//
//             if (accountWarehouseMap == null) throw new Exception();
//             
//             var akunPersediaan = accountWarehouseMap.AccountCode;
//             var akunBiaya = item.ItemType.AccountCode2;
//             
//             lines.Add(VoucherLine.Create(code, akunPersediaan, deliveryLine.Quantity * deliveryLine.Price, akunBiaya, akunPersediaan ));
//         }
//
//         if (delivery.Ppnamount > 0)
//             lines.Add(VoucherLine.Create(code, hutangPajak, 0, delivery.Ppnamount, ""));
//
//         var entry = Voucher.Create(
//             code,
//             prefix,
//             delivery.VoucherNo,
//             lines.ToArray()
//         );
//
//         await dbContext.Journals.AddAsync(entry, cancellationToken);
//     }
// }
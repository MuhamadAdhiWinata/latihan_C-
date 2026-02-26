using Integral.Api.Data.Contexts;
using Integral.Api.Features.GeneralLedger.Model;
using Integral.Api.Features.Sales.SalesInvoices.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Abstraction;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.GeneralLedger.EventHandler.Sales;

public class CreateVoucherOnInventoryApproved(PrintingDbContext dbContext, CurrentUserProvider currentUser) : INotificationHandler<SalesInvoiceApproved>
{
    public async Task Handle(SalesInvoiceApproved notification, CancellationToken cancellationToken)
    {
        const string piutangDagang = "10.01.03.01";
        const string hutangPajak = "20.01.01.03.01";
        const string prefix = "SJR";
         
        var user = currentUser.GetUsername();
        
        var codegen = new CodeGenerator(dbContext);
        var invoice = await dbContext.SalesInvoices
            .Where(x => x.Invno == notification.Code)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (invoice == null) throw new AppException("Invoice not found");

         
         var code = await codegen.GenerateAsync<Voucher>(prefix, nameof(Voucher.VoucherNo));
         
         var lines = new List<VoucherLine>
         {
             VoucherLine.Create(code, piutangDagang, invoice.GrandTotal, 0, "", user),
             VoucherLine.Create(code, invoice.BankAccountCode, 0, invoice.GrandTotal - invoice.Ppnamount, "", user)
         };

         if (invoice.Ppnamount > 0)
             lines.Add(VoucherLine.Create(code, hutangPajak, 0, invoice.Ppnamount, "", user));

         var entry = Voucher.Create(
             code,
             prefix,
             invoice.VoucherNo,
             user,
             lines.ToArray()
         );

         await dbContext.Journals.AddAsync(entry, cancellationToken);
    }
}
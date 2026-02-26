using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutationReceivings.Entities;
using Integral.Api.Features.Inventories.InventoryMutations.Events;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Inventories.InventoryMutationReceivings.EventHandlers;

public class SendPenerimaanOnMutationApproved(
    PublishingDbContext publishingDb,
    PrintingDbContext printingDb) : INotificationHandler<MutationTransferApproved>
{
    public async Task Handle(MutationTransferApproved notification, CancellationToken cancellationToken)
    {
        var transfer = await printingDb.MutationEntries
                           .Include(x => x.MutationItems)
                           .FirstOrDefaultAsync(x => x.TransactionCode == notification.Code, cancellationToken)
                       ?? throw new InvalidOperationException($"Transfer with code {notification.Code} not found.");

        var penerimaan = new PenerimaanMutasi
        {
            RefCode = transfer.TransactionCode,
            TransactionDate = transfer.TransactionDate,
            Status = MutationStatus.TRANSIT
        };

        foreach (var detail in notification.Details)
        {
            var masterItem =
                await printingDb.Items.FirstOrDefaultAsync(x => x.Code == detail.ItemCode, cancellationToken);

            if (masterItem == null)
                throw new InvalidOperationException($"Item {detail.ItemCode} not found.");

            var barang = await publishingDb.Barangs
                             .FirstOrDefaultAsync(x => x.KodeBarang == masterItem.Sku, cancellationToken)
                         ?? throw new InvalidOperationException($"Barang for item {detail.ItemCode} not found.");

            penerimaan.Details.Add(
                new PenerimaanMutasiDetail()
                {
                    BarangId = barang.Id.ToString(),
                    MutationItemId = detail.Id,
                    KodeProduksi = detail.ItemCode,
                    Quantity = detail.Quantity,
                    Harga = detail.Price,
                    HargaJual = detail.SellingPrice
                });

            penerimaan.TotalSellingPrice += detail.SellingPrice * detail.Quantity;
            penerimaan.GrandTotal += detail.Price * detail.Quantity;
        }

        await publishingDb.PenerimaanMutasis.AddAsync(penerimaan, cancellationToken);
    }
}
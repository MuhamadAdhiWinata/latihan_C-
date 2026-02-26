using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Barangs.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Barangs.Features;

public record AddStockCommand(string TransactionCode, string ItemCode, int Quantity) : IRequest<Unit>;

public class AddStockCommandHandler(PublishingDbContext publishingDb) : IRequestHandler<AddStockCommand, Unit>
{
    public async Task<Unit> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        var barang =
            await publishingDb.Barangs.FirstOrDefaultAsync(x => x.KodeBarang == request.ItemCode, cancellationToken);
        if (barang == null)
            throw new InvalidOperationException($"Item {request.ItemCode} not found.");

        var stock = new BarangDetail
        {
            BarangId = barang.Id,
            KodeBarang = barang.KodeBarang,
            Stok = request.Quantity.ToString(),
            StokIn = request.Quantity.ToString(),
            HargaBeli = 0,
            CreatedAt = DateTime.Now,
            Ket = $"PENGIRIMAN {request.TransactionCode}",
            JenisBarang = "INTERCOMPANY"
        };

        await publishingDb.BarangDetails.AddAsync(stock, cancellationToken);

        return Unit.Value;
    }
}
using Integral.Api.Features.Inventories.InventoryMutationReceivings.Entities;
using Integral.Api.Features.Master.Barangs.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Ef;

namespace Integral.Api.Data.Contexts;

public class PublishingDbContext(DbContextOptions<PublishingDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Barang> Barangs { get; set; }
    public DbSet<BarangDetail> BarangDetails { get; set; }

    public DbSet<PenerimaanMutasi> PenerimaanMutasis { get; set; }
    public DbSet<PenerimaanMutasiDetail> PenerimaanMutasiDetails { get; set; }
}
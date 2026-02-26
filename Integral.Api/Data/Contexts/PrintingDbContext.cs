using Integral.Api.Features.GeneralLedger.Model;
using Integral.Api.Features.Identity.Models;
using Integral.Api.Features.Inventories.InventoryIns.Models;
using Integral.Api.Features.Inventories.InventoryMutations.Models;
using Integral.Api.Features.Inventories.InventoryOuts.Entities;
using Integral.Api.Features.Inventories.Stocks.Models;
using Integral.Api.Features.Inventories.WarehouseMutations.Entities;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;
using Integral.Api.Features.Manufacturing.WorkOrderOuts.Models;
using Integral.Api.Features.Manufacturing.WorkOrders.Entities;
using Integral.Api.Features.Master.Entities;
using Integral.Api.Features.Master.Items.Models;
using Integral.Api.Features.Sales.SalesDeliveries.Entities;
using Integral.Api.Features.Sales.SalesInvoices.Entities;
using Integral.Api.Features.Sales.SalesOrders.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Ef;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Data.Contexts;

public class PrintingDbContext(DbContextOptions<PrintingDbContext> options, CurrentUserProvider currentUserProvider)
    : DbContext(options), IAppDbContext
{
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemType> ItemTypes { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Account> Accounts { get; set; }


    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkOrderItem> WorkOrderItems { get; set; }

    public DbSet<WorkOrderMaterial> WorkOrderMaterials { get; set; }

    public DbSet<WorkOrderMachine> WorkOrderMachines { get; set; }

    public DbSet<WorkOrderLabor> WorkOrderLabors { get; set; }

    public DbSet<WorkOrderFinishing> WorkOrderFinishings { get; set; }

    public DbSet<WorkOrderPackaging> WorkOrderPackagings { get; set; }
    public DbSet<WorkOrderOut> WorkOrderOuts { get; set; }
    public DbSet<WorkOrderOutItem> WorkOrderOutItems { get; set; }
    public DbSet<WorkOrderInput> WorkOrderInputs { get; set; }
    public DbSet<WorkOrderInputItem> WorkOrderInputItems { get; set; }

    #region Inventory

    public DbSet<F303> F303s { get; set; }
    public DbSet<F304> F304s { get; set; }

    public DbSet<F309> F309s { get; set; }
    public DbSet<F310> F310s { get; set; }

    #endregion


    public DbSet<StockBatch> StockBatches { get; set; }
    public DbSet<StockReservation> StockReservations { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }

    public DbSet<MutationEntry> MutationEntries { get; set; }
    public DbSet<MutationItem> MutationItems { get; set; }


    public DbSet<Order> F603s { get; set; }
    public DbSet<OrderItem> F604s { get; set; }
    public DbSet<F604b> F604bs { get; set; }
    public DbSet<F604c> F604cs { get; set; }
    public DbSet<F604d> F604ds { get; set; }
    public DbSet<F604e> F604es { get; set; }
    public DbSet<F604f> F604fs { get; set; }


    public DbSet<Voucher> Journals { get; set; }
    public DbSet<VoucherLine> JournalDetails { get; set; }
    public DbSet<AccountWarehouse> F015Bs { get; set; }

    public DbSet<SalesDelivery> SalesDeliveries { get; set; }
    public DbSet<SalesDeliveryLine> SalesDeliveriyLines { get; set; }
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceLine> SalesInvoiceLines { get; set; }
    public DbSet<SalesInvoiceDelivery> SalesInvoiceDeliveries { get; set; }
    public DbSet<InventoryOut> InventoryOuts { get; set; }
    public DbSet<InventoryOutLine> InventoryOutLines { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VoucherLine>()
            .HasOne(d => d.Header)
            .WithMany(h => h.Lines)
            .HasForeignKey(d => new { d.VoucherNo, d.BranchCode })
            .HasPrincipalKey(h => new { h.VoucherNo, h.BranchCode });

        modelBuilder.Entity<StockTransaction>()
            .HasOne(x => x.ReversalOf)
            .WithOne(x => x.ReversedBy)
            .HasForeignKey<StockTransaction>(x => x.ReverseOfId)
            .OnDelete(DeleteBehavior.Restrict);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(decimal) && property.ClrType != typeof(decimal?)) continue;

                property.SetPrecision(18);
                property.SetScale(4);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaving()
    {
        try
        {
            var username = currentUserProvider?.GetUsername() ?? "";

            foreach (var entry in ChangeTracker.Entries<ICreatedAuditable>())
            {
                if (entry.State != EntityState.Added) continue;

                entry.Entity.CreatedDate = DateTime.Now;
                entry.Entity.CreatedBy = username;
            }

            foreach (var entry in ChangeTracker.Entries<IUpdatedAuditable>())
            {
                if (entry.State != EntityState.Modified) continue;

                entry.Entity.UpdatedDate = DateTime.Now;
                entry.Entity.UpdatedBy = username;
            }
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("try for find IAggregate", ex);
        }
    }
}
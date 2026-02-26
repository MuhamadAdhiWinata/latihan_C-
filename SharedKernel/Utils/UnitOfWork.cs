using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel;

public class UnitOfWork(IEnumerable<DbContext> dbContexts)
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var totalChanges = 0;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        // foreach (var context in dbContexts)
        // {
        //     foreach (var entity in context.ChangeTracker.Entries().Where(e => e.Entity is Aggregate))
        //     {
        //         
        //     }
        // }

        foreach (var context in dbContexts) totalChanges += await context.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return totalChanges;
    }
}
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Abstraction.Ef;

public class UnitOfWork(IEnumerable<IAppDbContext> dbContexts)
{
    private readonly IEnumerable<DbContext> _dbContexts = dbContexts.Cast<DbContext>();

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var result = 0;
        foreach (var db in _dbContexts)
        {
            result += await db.SaveChangesAsync(cancellationToken);
        }
        scope.Complete();
        return result;
    }
}

public static class UnitOfWorkExtensions
{
    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>();
    }
}
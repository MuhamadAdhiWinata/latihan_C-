using Integral.Api.Data.Contexts;
using Integral.Api.Features.Inventories.InventoryMutations.Exceptions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;

namespace Integral.Api.Features.Master.Barangs.Features;

public record FindByCodeBarang(string Code) : IQuery<FindByCodeBarangResult>;

public record FindByCodeBarangResult(BarangDto Data);

public class FindByCodeBarangHandler(PublishingDbContext dbContext) : IQueryHandler<FindByCodeBarang, FindByCodeBarangResult>
{
    public async Task<FindByCodeBarangResult> Handle(FindByCodeBarang request, CancellationToken cancellationToken)
    {
        var res = await dbContext.Barangs
            .AsNoTracking()
            .Where(o => o.KodeBarang == request.Code)
            .Select(o => new BarangDto(
                (long)o.Id,
                o.KodeBarang,
                o.NamaBarang
            ))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (res == null)
            throw new BarangNotFoundException(request.Code);
        
        return new FindByCodeBarangResult(res);
    }
}
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Items.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Items.Features.Services;

public static class ItemMapper
{
    public static ItemResultDto ToDto(this Item item)
    {
        return new ItemResultDto(
            item.Code,
            item.Name,
            item.UnitCode,
            item.Price,
            item.Sku,
            item.ItemTypeCode,
            item.ItemType.Name,
            item.ActiveStatus,
            item.MerkCode
        );
    }
}

public class ItemQueryService(PrintingDbContext printingDb, CurrentUserProvider currentUser) : IItemQueryService
{
    public async Task<FindByCodeResult> FindByCode(string code, CancellationToken cancellationToken = default)
    {
        var res = await printingDb.Items
            .Include(x => x.ItemType)
            .Where(x => x.Code == code)
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        return new FindByCodeResult(res);
    }

    public async Task<FindByCodesResult> FindByCodes(string[] codes, CancellationToken cancellationToken = default)
    {
        var res = await printingDb.Items
            .Include(x => x.ItemType)
            .Where(x => codes.Contains(x.Code))
            .Select(x => x.ToDto())
            .ToArrayAsync(cancellationToken);

        return new FindByCodesResult(res);
    }


    public async Task<FindAllItemResult> FindAll(FindAllItem request, CancellationToken cancellationToken = default)
    {
        var baseQuery = printingDb.Items
            .AsNoTracking()
            .Include(i => i.ItemType)
            .OrderBy(i => i.Code)
            .AsQueryable();

        switch (request.Order)
        {
            case OrderDirection.Ascending:
                baseQuery = baseQuery.OrderBy(i => i.CreatedDate);
                break;

            case OrderDirection.Descending:
                baseQuery = baseQuery.OrderByDescending(i => i.CreatedDate);
                break;
        }

        if (!string.IsNullOrWhiteSpace(request.Search.Trim()))
            baseQuery = baseQuery.Where(o =>
                o.Code.Contains(request.Search.Trim()) || o.Name.Contains(request.Search.Trim()));

        if (request.HasSku.HasValue)
            baseQuery = baseQuery.Where(o => !string.IsNullOrWhiteSpace(o.Sku) == request.HasSku.Value);

        var count = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Select(x => x.ToDto())
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new FindAllItemResult(count, items.ToArray());
    }

    public async Task Create(ItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingItem = await printingDb.Items.FirstOrDefaultAsync(x => x.Code == request.ItemCode, cancellationToken);
        if (existingItem != null)
            throw new InvalidOperationException($"Item with {request.ItemCode} already exist.");

        var user = currentUser.GetUsername();
        var item = new Item
        {
            Code = request.ItemCode,
            Name = request.ItemName,
            UnitCode = request.UnitCode,
            Price = request.Price,
            Sku = request.Sku,
            ItemTypeCode = request.ItemTypeCode,
            ActiveStatus = request.ActiveStatus,
            MerkCode = request.MerkCode,
            CreatedBy = user,
            CreatedDate = DateTime.Now,
            UpdatedBy = "",
            UpdatedDate = DateTime.Now
        };

        await printingDb.Items.AddAsync(item, cancellationToken);
        await printingDb.SaveChangesAsync(cancellationToken);
    }

    public async Task Modify(ItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var item = await printingDb.Items.FirstOrDefaultAsync(x => x.Code == request.ItemCode, cancellationToken);
        if (item == null)
            throw new InvalidOperationException($"Item {request.ItemCode} not found.");

        item.Name = request.ItemName;
        item.UnitCode = request.UnitCode;
        item.Price = request.Price;
        item.Sku = request.Sku;
        item.ItemTypeCode = request.ItemTypeCode;
        item.ActiveStatus = request.ActiveStatus;
        item.MerkCode = request.MerkCode;

        await printingDb.SaveChangesAsync(cancellationToken);
    }
}

public class ItemServiceEndpoints(ItemQueryService queryService) : IMinimalEndpoint
{
    private const string BaseApiPath = $"{MasterRoot.BaseApiPath}/items";

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{BaseApiPath}",
                async ([AsParameters] FindAllItem query) => await queryService.FindAll(query))
            .RequireAuthorization()
            .WithTags("Master Items")
            .WithName("Find All Item Master");

        return builder;
    }
}
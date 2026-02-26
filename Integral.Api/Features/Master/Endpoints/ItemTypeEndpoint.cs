using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Endpoints;

public record ItemTypeDto(string Code, string Name, string AccountCode, string AccountCode2, bool IsActive);

public record ItemTypePostRequest(string Code, string Name, string AccountCode);

public record ItemTypePutRequest(string? Code, string? Name, string? AccountCode);

public static class ItemTypeMap
{
    public static ItemTypeDto ToDto(this ItemType itemType)
    {
        var dto = new ItemTypeDto(
            itemType.Code,
            itemType.Name,
            itemType.AccountCode,
            itemType.AccountCode2,
            itemType.ActiveStatus > 0);

        return dto;
    }
}

public class ItemTypeEndpoint(PrintingDbContext dbContext) : IMinimalEndpoint
{
    private const string BaseApiPath = $"{MasterRoot.BaseApiPath}/item-types";

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseApiPath).WithTags("Master Item Types").RequireAuthorization();

        group.MapGet("", async (int limit = 100, int offset = 0, string? search = null) =>
        {
            var query = dbContext.ItemTypes
                .AsNoTracking()
                .OrderBy(x => x.Code).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.Code.Contains(search) ||
                    x.Name.Contains(search));

            var res = await query.Skip(offset).Take(limit).Select(x => x.ToDto()).ToListAsync();

            return Results.Ok(new
            {
                count = await query.CountAsync(),
                data = res
            });
        });

        group.MapGet("/{code}", async (string code) =>
        {
            var itemType = await dbContext.ItemTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code);

            return itemType is null
                ? Results.NotFound()
                : Results.Ok(itemType.ToDto());
        });

        group.MapPost("", async (ItemTypePostRequest request) =>
        {
            var itemType = new ItemType
            {
                Code = request.Code,
                Name = request.Name,
                AccountCode = request.AccountCode
            };

            await dbContext.ItemTypes.AddAsync(itemType);
            await dbContext.SaveChangesAsync();
            return Results.Ok(itemType.ToDto());
        });

        group.MapPut("{code}", async (ItemTypePutRequest request, string code) =>
        {
            var itemType = await dbContext.ItemTypes.FirstOrDefaultAsync(x => x.Code == code);
            if (itemType == null) return Results.NotFound();

            itemType.Code = request.Code ?? itemType.Code;
            itemType.Name = request.Name ?? itemType.Name;
            itemType.AccountCode = request.AccountCode ?? itemType.AccountCode;

            await dbContext.SaveChangesAsync();
            return Results.Ok(itemType.ToDto());
        });

        return builder;
    }
}
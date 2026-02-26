using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Endpoints;

public record WarehouseDto(string Code, string Name, string AccountCode);

public record WarehousePostRequest(string Code, string Name, string AccountCode);

public record WarehousePutRequest(string? Code, string? Name, string? AccountCode);

public static class WarehouseMapper
{
    public static WarehouseDto ToDto(this Warehouse warehouse)
    {
        return new WarehouseDto(warehouse.Code, warehouse.Name, warehouse.AccountCode);
    }
}

public class WarehouseEndpoint(PrintingDbContext dbContext) : IMinimalEndpoint
{
    private const string BaseApiPath = $"{MasterRoot.BaseApiPath}/warehouses";

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseApiPath).WithTags("Master Warehouses").RequireAuthorization();

        group.MapGet("", async (int limit = 100, int offset = 0, string? search = null) =>
        {
            var query = dbContext.Warehouses
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
            var warehouse = await dbContext.Warehouses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code);

            return warehouse is null
                ? Results.NotFound()
                : Results.Ok(warehouse.ToDto());
        });

        group.MapPost("", async (WarehousePostRequest request) =>
        {
            var warehouse = new Warehouse
            {
                Code = request.Code,
                Name = request.Name,
                AccountCode = request.AccountCode
            };

            await dbContext.Warehouses.AddAsync(warehouse);
            await dbContext.SaveChangesAsync();
            return Results.Ok(warehouse.ToDto());
        });

        group.MapPut("{code}", async (WarehousePutRequest request, string code) =>
        {
            var warehouse = await dbContext.Warehouses.FirstOrDefaultAsync(x => x.Code == code);
            if (warehouse == null) return Results.NotFound();

            warehouse.Code = request.Code ?? warehouse.Code;
            warehouse.Name = request.Name ?? warehouse.Name;
            warehouse.AccountCode = request.AccountCode ?? warehouse.AccountCode;

            await dbContext.SaveChangesAsync();
            return Results.Ok(warehouse.ToDto());
        });

        return builder;
    }
}
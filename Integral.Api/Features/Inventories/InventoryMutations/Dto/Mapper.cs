using Integral.Api.Features.Inventories.InventoryMutations.Models;
using Integral.Api.Features.Inventories.Stocks.Models;

namespace Integral.Api.Features.Inventories.InventoryMutations.Dto;

public record MutationDto(
    MutationHeaderDto Header,
    MutationLineDto[] Lines,
    string[] WorkOrders
);

public record MutationHeaderDto(
    int Id,
    string TransactionCode,
    DateTime TransactionDate,
    string WarehouseCode,
    string? RefNo,
    string Description,
    string Status,
    string CreatedBy
);

public record MutationLineDto(
    long Id,
    string Code,
    string Name,
    string? Sku,
    decimal Quantity,
    decimal SellingPrice,
    MutationLineDetailDto[] Detail
);

public record MutationLineDetailDto(
    int BatchId,
    long MovementId,
    decimal Quantity,
    decimal Cost
);

public static class Mapper
{
    public static MutationLineDto ToDto(this MutationItem entity, StockTransaction[] transactions)
    {
        var details = transactions.Select(x => new MutationLineDetailDto(
            x.StockBatch.Id,
            x.Id,
            x.Out,
            x.StockBatch.Cogsidr
        ));

        return new MutationLineDto(entity.Id, entity.ItemCode, entity.ItemName, entity.ItemNavigation?.Sku ?? "",
            entity.Quantity, entity.SellingPrice, details.ToArray());
    }

    public static MutationHeaderDto ToDto(this MutationEntry entity, string? author = null)
    {
        return new MutationHeaderDto(
            entity.Id,
            entity.TransactionCode,
            entity.TransactionDate,
            entity.WarehouseCode,
            entity.RefNo,
            entity.Description,
            entity.Status,
            (author ?? entity.CreatedBy) ?? ""
        );
    }
}
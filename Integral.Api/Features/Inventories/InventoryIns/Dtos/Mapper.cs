using Integral.Api.Features.Inventories.InventoryIns.Models;

namespace Integral.Api.Features.Inventories.InventoryIns.Dtos;

public record InventoryInHeaderDto(
    string BranchCode,
    string Code,
    DateTime TransactionDate,
    string RefNo,
    string CurrencyCode,
    decimal ExchangeRate,
    string WareHouseCode,
    string Description,
    decimal TotalTransactionAmount,
    short DeleteStatus,
    string CreatedBy,
    DateTime CreatedDate,
    string UpdatedBy,
    DateTime UpdatedDate,
    bool PostIin
);

public record InventoryInLineDto(
    long Id,
    string BranchCode,
    string Code,
    string ItemCode,
    string ItemAlias,
    decimal Quantity,
    decimal Price,
    string? ReasonCode,
    string Description
);

public record InventoryInDtoNew(
    InventoryInHeaderDto Header,
    InventoryInLineDto[] Line
);

public static class Mapper
{
    public static InventoryInHeaderDto ToDto(this F303 f)
    {
        return new InventoryInHeaderDto(
            f.BranchCode,
            f.Iinno,
            f.TransactionDate,
            f.RefNo,
            f.CurrencyCode,
            f.ExchangeRate,
            f.WareHouseCode,
            f.Description,
            f.TotalTransactionAmount,
            f.DeleteStatus,
            f.CreatedBy,
            f.CreatedDate,
            f.UpdatedBy,
            f.UpdatedDate,
            f.PostIin
        );
    }

    public static InventoryInLineDto ToDto(this F304 d)
    {
        return new InventoryInLineDto(
            d.Id,
            d.BranchCode,
            d.Iinno,
            d.ItemCode,
            d.ItemCodeNavigation.Name,
            d.Quantity,
            d.Price,
            d.ReasonCode,
            d.Description
        );
    }
}
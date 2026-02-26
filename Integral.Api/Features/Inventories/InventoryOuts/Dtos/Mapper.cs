using Integral.Api.Features.Inventories.InventoryOuts.Entities;

namespace Integral.Api.Features.Inventories.InventoryOuts.Dtos;

public static class Mapper
{
    public static InventoryOutHeaderDto ToDto(this InventoryOut entity)
    {
        return new InventoryOutHeaderDto(
            entity.Iotno,
            entity.BranchCode,
            entity.TransactionDate,
            entity.RefNo,
            entity.CurrencyCode,
            entity.ExchangeRate,
            entity.WareHouseCode,
            entity.Description
        );
    }

    public static InventoryOutLineDto ToDto(this InventoryOutLine entity)
    {
        return new InventoryOutLineDto(
            entity.Iotno,
            entity.BranchCode,
            entity.ItemCode,
            entity.ItemNavigation.Name,
            entity.Quantity,
            entity.Price,
            entity.ReasonCode,
            entity.Description
        );
    }
}
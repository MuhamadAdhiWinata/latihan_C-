using Integral.Api.Features.Inventories.WarehouseMutations.Entities;

namespace Integral.Api.Features.Inventories.WarehouseMutations.Dto;

public record WarehouseMutationHeaderDto
(
    string Code,
    string BranchCode,
    DateTime TransactionDate,
    decimal TotalTransactionAmount,
    string SourceWarehouseCode,
    string DestinationWarehouseCode,
    string RefNo,
    string Description,
    bool Approved,
    string CreatedBy,
    DateTime CreatedDate,
    string UpdatedBy,
    DateTime UpdatedDate
);

public record WarehouseMutationLineDto
(
    string WhmCode,
    string ItemCode,
    decimal Quantity,
    decimal Price,
    decimal Total,
    string Description,
    string CreatedBy,
    DateTime CreatedDate,
    string UpdatedBy,
    DateTime UpdatedDate
);


public static class Mapper
{
    public static WarehouseMutationHeaderDto ToDto(
        this F309 entity
    )
    {
        return new WarehouseMutationHeaderDto(
            Code: entity.Whmno,
            BranchCode: entity.BranchCode,
            TransactionDate: entity.TransactionDate,
            TotalTransactionAmount: entity.TotalTransactionAmount,
            SourceWarehouseCode: entity.WareHouseSourceCode,
            DestinationWarehouseCode: entity.WareHouseDestinationCode,
            RefNo: entity.RefNo,
            Description: entity.Description,
            Approved: entity.Approved,
            CreatedBy: entity.CreatedBy,
            CreatedDate: entity.CreatedDate,
            UpdatedBy: entity.UpdatedBy,
            UpdatedDate: entity.UpdatedDate
        );
    }

    public static WarehouseMutationLineDto ToDto(this F310 entity)
    {
        return new WarehouseMutationLineDto(
            WhmCode: entity.Whmno,
            ItemCode: entity.ItemCode,
            Quantity: entity.Quantity,
            Price: entity.Price,
            Total: entity.Total,
            Description: entity.Description,
            CreatedBy: entity.CreatedBy,
            CreatedDate: entity.CreatedDate,
            UpdatedBy: entity.UpdatedBy,
            UpdatedDate: entity.UpdatedDate
        );
    }
}
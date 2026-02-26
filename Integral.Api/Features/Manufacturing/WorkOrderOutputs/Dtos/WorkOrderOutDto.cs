namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Dtos;

// public record WorkOrderOutDto(
//     string Code,
//     DateTime TransactionDate,
//     string WorkOrderCode,
//     string Description,
//     decimal TotalTransactionAmount,
//     string Status,
//     string CreatedBy,
//     DateTime CreatedDate,
//     string? UpdatedBy,
//     DateTime? UpdatedDate
// );
//
// public record WorkOrderOutItemDto(
//     string ItemCode,
//     string ItemName,
//     string Sku,
//     decimal Quantity,
//     decimal Price,
//     decimal HppAktual,
//     string Description
// );
//
// public record WorkOrderOutHistoryDto(
//     string Code,
//     DateTime TransactionDate,
//     string Description,
//     string WorkOrderCode,
//     decimal TotalTransactionAmount,
//     string Status,
//     string CreatedBy,
//     DateTime CreatedDate,
//     string? UpdatedBy,
//     DateTime? UpdatedDate,
//     
//     WorkOrderOutItemHistoryDto[] Items
// );
//
// public record WorkOrderOutItemHistoryDto(
//     string ItemCode,
//     string ItemName,
//     string Sku,
//     decimal Quantity,
//     decimal UnfinishedQuantity,
//     decimal Price,
//     decimal HppAktual,
//     string Description
// )
// {
//     public decimal UnfinishedQuantity { get; set; }
// };
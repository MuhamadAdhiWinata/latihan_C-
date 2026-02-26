using Integral.Api.Features.Manufacturing.WorkOrderOuts.Models;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Dtos;

public record WorkOrderOutHeaderDto(
    string Code,
    DateTime TransactionDate,
    string WorkOrderCode,
    string Description,
    decimal TotalTransactionAmount,
    string Status,
    string CreatedBy,
    DateTime CreatedDate,
    string? UpdatedBy,
    DateTime? UpdatedDate
);

public record WorkOrderOutLineDto(
    string ItemCode,
    string ItemName,
    string Sku,
    decimal Quantity,
    decimal HppEstimasi,
    decimal Hpp,
    string Description
)
{
    public decimal? UnfinishedQuantity { get; set; } = null;
};

public record WorkOrderOutDto(
    WorkOrderOutHeaderDto Header,
    WorkOrderOutLineDto[] Lines
);

public static class Mapper
{
    public static WorkOrderOutHeaderDto ToDto(this WorkOrderOut entity)
    {
        return new WorkOrderOutHeaderDto(
            entity.Code,
            entity.TransactionDate,
            entity.WorkOrderCode,
            entity.Description,
            entity.TotalTransactionAmount,
            entity.Status,
            entity.CreatedBy,
            entity.CreatedDate,
            entity.UpdatedBy,
            entity.UpdatedDate
            
            
        );
    }

    public static WorkOrderOutLineDto ToDto(this WorkOrderOutItem line)
    {
        return new WorkOrderOutLineDto(
            line.ItemCode,
            line.ItemNavigation.Name ?? string.Empty,
            line.ItemNavigation.Sku ?? string.Empty,
            line.Quantity,
            line.Price,
            line.HppAktual ,
            line.Description);
    }
}

using Integral.Api.Features.Manufacturing.WorkOrderInputs.Entities;

namespace Integral.Api.Features.Manufacturing.WorkOrderInputs.Dtos;

public record WorkOrderInputHeaderDto(
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

public record WorkOrderInputLineDto(
    string ItemCode,
    string ItemName,
    string Sku,
    decimal Quantity,
    decimal Price,
    string Description
)
{
    public decimal? QuantitySent { get; set; } = null; 
};

public record WorkOrderInputDto(
    WorkOrderInputHeaderDto Header,
    WorkOrderInputLineDto[] Lines
);

public static class Mapper
{
    public static WorkOrderInputHeaderDto ToDto(this WorkOrderInput entity)
    {
        return new WorkOrderInputHeaderDto(
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

    public static WorkOrderInputLineDto ToDto(this WorkOrderInputItem line)
    {
        return new WorkOrderInputLineDto(
            line.ItemCode,
            line.ItemNavigation.Name ?? string.Empty,
            line.ItemNavigation.Sku ?? string.Empty,
            line.Quantity,
            line.Price,
            line.Description);
    }
}
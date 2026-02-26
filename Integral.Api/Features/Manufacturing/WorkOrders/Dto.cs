namespace Integral.Api.Features.Manufacturing.WorkOrders;

public record WorkOrderDto(
    string BranchCode,
    string Code,
    DateTime TransactionDate,
    string CustomerCode,
    string SalesOrderCode,
    string Description,
    string? Prepressinstruction,
    string? Pressinstruction,
    string? Finishinginstruction,
    string? Packaginginstruction,
    string Status,
    short DeleteStatus,
    string Spdescription,
    bool? Approved,
    bool? Approved2,
    short Isprepress,
    short Ispress,
    short Isfinishing,
    short Ispackaging
);

public record WorkOrderItemDto(
    string BranchCode,
    string Dodno,
    string ItemCode,
    string ItemAlias,
    decimal Quantity,
    decimal UnfinishedQuantity,
    decimal Price,
    string Description,
    decimal DiscountPercent,
    decimal DiscountPercent1,
    decimal DiscountPercent2,
    decimal DiscountPercent3,
    decimal DiscountAmount1,
    decimal DiscountAmount2,
    decimal DiscountAmount3,
    string CreatedBy,
    DateTime CreatedDate,
    string UpdatedBy,
    DateTime UpdatedDate,
    decimal BonusQuantity,
    string? AlterdBy,
    DateTime? AlterdDate
);

public record WorkOrderMaterialDto(
    string BranchCode,
    string Spk,
    string ItemCode,
    string ItemAlias,
    decimal Quantity,
    decimal QuantitySent,
    decimal Price,
    string Description,
    string CreatedBy,
    DateTime CreatedDate,
    string UpdatedBy,
    DateTime UpdatedDate
);

public record WorkOrderDetailDto(
    string BranchCode,
    string Spk,
    string ItemCode,
    string ItemAlias,
    decimal Quantity,
    decimal Price,
    string Description,
    string CreatedBy,
    DateTime CreatedDate,
    string UpdatedBy,
    DateTime UpdatedDate
);
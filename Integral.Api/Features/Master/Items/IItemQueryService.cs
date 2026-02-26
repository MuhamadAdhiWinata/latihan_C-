namespace Integral.Api.Features.Master.Items;

public interface IItemQueryService
{
    public Task<FindByCodeResult> FindByCode(string code, CancellationToken cancellationToken = default);
    public Task<FindByCodesResult> FindByCodes(string[] codes, CancellationToken cancellationToken = default);
    public Task<FindAllItemResult> FindAll(FindAllItem request, CancellationToken cancellationToken = default);
}

public record ItemRequestDto(
    string ItemCode,
    string ItemName,
    string UnitCode,
    decimal Price,
    string? Sku,
    string ItemTypeCode,
    short ActiveStatus,
    string MerkCode);

public record ItemModifyRequestDto(
    string ItemName,
    string UnitCode,
    decimal Price,
    string? Sku,
    string ItemTypeCode,
    short ActiveStatus,
    string MerkCode);


public record ItemResultDto(
    string ItemCode,
    string ItemName,
    string UnitCode,
    decimal Price,
    string? Sku,
    string ItemTypeCode,
    string ItemTypeName,
    short ActiveStatus,
    string MerkCode
);

public record FindByCodeResult(ItemResultDto? Data);

public record FindByCodesResult(ItemResultDto[] Data);

public record FindAllItemResult(int Count, ItemResultDto[] Data);

public record FindAllItem(
    int Limit = 100,
    int Offset = 0,
    string Search = "",
    bool? HasSku = null,
    OrderDirection? Order = null
);

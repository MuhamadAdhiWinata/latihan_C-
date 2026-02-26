using Integral.Api.Features.GeneralLedger.Model;

namespace Integral.Api.Features.GeneralLedger.Dto;

public static class VoucherStatus
{
    public const string Pending = "pending";
    public const string Approved = "approved";
}

public record VoucherDto(string Code, string RefNo, DateTime TransactionDate, string Type, string Status, VoucherLineDto[] Lines);

public record VoucherLineDto(long Id, string AccountCode, decimal Debit, decimal Credit, string Description);

public static class Mapper
{
    public static VoucherDto ToDto(this Voucher entity)
    {
        return new VoucherDto(
            entity.VoucherNo,
            entity.RefNo,
            entity.TransactionDate,
            entity.JournalType,
            entity.PostingStatus > 0 ? VoucherStatus.Approved : VoucherStatus.Pending,
            entity.Lines.Select(x => x.ToDto()).ToArray()
        );
    }

    private static VoucherLineDto ToDto(this VoucherLine entity)
    {
        return new VoucherLineDto(
            entity.Id,
            entity.AccountCode,
            entity.Debit,
            entity.Credit,
            entity.Description);
    }
}
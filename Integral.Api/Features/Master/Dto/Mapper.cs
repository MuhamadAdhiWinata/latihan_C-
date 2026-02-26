using Integral.Api.Features.Master.Entities;

namespace Integral.Api.Features.Master.Dto;

public record AccountDto(
    string Code,
    string Name,
    string Level,
    string Type
);

public static class Mapper
{
    public static AccountDto ToDto(this Account entity)
    {
        return new AccountDto(entity.Code, entity.Name, entity.Level, entity.Type);
    }
}
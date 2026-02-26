using Integral.Api.Features.Identity.Models;

namespace Integral.Api.Features.Identity.Dtos;

public record RoleDto(
    long Id,
    string Code,
    string Name
);

public record UserDto(
    string UserName,
    string RoleCode,
    string Name
);

public static class Mapper
{
    public static RoleDto ToDto(this Role role)
    {
        return new RoleDto(
            role.Id,
            role.Code,
            role.Name
        );
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Code,
            user.GroupCode,
            user.Name ?? ""
        );
    }
}
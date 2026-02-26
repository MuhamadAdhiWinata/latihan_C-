namespace Integral.Api.Features.Identity.Models;

public class RoleClaim
{
    public long Id { get; set; }
    public long RoleId { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}
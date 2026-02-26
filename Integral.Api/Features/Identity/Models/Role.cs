namespace Integral.Api.Features.Identity.Models;

public class Role
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }

    public virtual ICollection<RoleClaim> Claims { get; set; }
}
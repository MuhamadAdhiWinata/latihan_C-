using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Identity.Models;

[Table("f102")]
[Index("Code", Name = "UQ_f102_Code", IsUnique = true)]
public class User
{
    [Key] public long Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Name { get; set; }

    public string GroupCode { get; set; } = null!;

    public short ActiveStatus { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedDate { get; set; }

    // [InverseProperty("User")]
    // public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    private string Hash(string password)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = md5.ComputeHash(inputBytes);
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        var trimmedHash = hash[..^2];
        return trimmedHash;
    }

    public bool VerifyPassword(string password)
    {
        var hash = Hash(password);
        return hash == Password;
    }

    public void SetPassword(string password)
    {
        var hash = Hash(password);
        Password = hash;
    }
}
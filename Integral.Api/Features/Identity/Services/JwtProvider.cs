using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;

namespace Integral.Api.Features.Identity.Services;

public class JwtProvider(JwtSettings settings)
{
    private SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(settings.Key));

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            claims,
            signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha512));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginApp.Services.Security;

public class TokenService
{
    public string GetAccessToken(Claim[] claims)
    {
        Claim[] jwtClaim =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, DateTime.UtcNow.ToString()),
        ];

        var jwtCLaims = claims.Concat(jwtClaim);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configurations.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );


        var token = new JwtSecurityToken(
            null,
            null,
            jwtCLaims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
            );

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }
}

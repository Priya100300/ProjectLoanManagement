using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Services;

/// <summary>
/// Issues the "ID card" (JWT) after a successful login. The token carries userId, name and role,
/// and is signed with the secret key so nobody can change it without being detected.
/// </summary>
public class JwtTokenService
{
    public const string UserIdClaim = "userId";
    public const string NameClaim = "name";
    public const string RoleClaim = "role";

    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public LoginResponse CreateToken(User user)
    {
        DateTime expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(UserIdClaim, user.UserId.ToString()),
            new Claim(NameClaim, user.UserName),
            new Claim(RoleClaim, user.Role)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expires,
            UserId = user.UserId,
            UserName = user.UserName,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}

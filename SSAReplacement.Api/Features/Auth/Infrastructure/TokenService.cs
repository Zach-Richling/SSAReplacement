using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SSAReplacement.Api.Infrastructure;
using RefreshTokenEntity = SSAReplacement.Api.Domain.RefreshToken;

namespace SSAReplacement.Api.Features.Auth.Infrastructure;

public class TokenService(IOptions<JwtSettings> jwtOptions, AppDbContext db)
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public string GenerateAccessToken(string username, long? userId, out DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new(JwtRegisteredClaimNames.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (userId.HasValue)
            claims.Add(new Claim("uid", userId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshTokenEntity> GenerateRefreshTokenAsync(string username, long? userId)
    {
        var refreshToken = new RefreshTokenEntity
        {
            Username = username,
            UserId = userId,
            Token = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays)
        };

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshTokenEntity?> GetValidRefreshTokenAsync(string token)
    {
        var refreshToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken is null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        return refreshToken;
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenEntity token, string? replacedByToken = null)
    {
        token.IsRevoked = true;
        token.ReplacedByToken = replacedByToken;
        await db.SaveChangesAsync();
    }
}

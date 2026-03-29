using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Domain;
using SSAReplacement.Api.Features.Auth.Domain;
using SSAReplacement.Api.Features.Auth.Infrastructure;
using SSAReplacement.Api.Infrastructure;
using System.Security.Claims;
using System.Security.Principal;

namespace SSAReplacement.Api.Features.Auth.Handlers;

public static class GetToken
{
    public static async Task<IResult> Handler(ClaimsPrincipal user, TokenService tokenService, AppDbContext db)
    {
        var username = user.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Results.Unauthorized();

        var sid = (user.Identity as WindowsIdentity)?.User?.Value;
        if (string.IsNullOrEmpty(sid))
            return Results.Unauthorized();

        var dbUser = await db.Users.FirstOrDefaultAsync(u => u.Sid == sid);
        if (dbUser is null)
        {
            dbUser = new User { Sid = sid, Username = username };
            db.Users.Add(dbUser);
        }
        else
        {
            dbUser.Username = username;
            dbUser.LastSeenAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();

        var accessToken = tokenService.GenerateAccessToken(username, dbUser.Id, out var expiresAt);
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(username, dbUser.Id);

        return Results.Ok(new TokenResponse(accessToken, refreshToken.Token, expiresAt));
    }
}

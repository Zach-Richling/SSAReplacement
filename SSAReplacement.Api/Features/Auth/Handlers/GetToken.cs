using SSAReplacement.Api.Features.Auth.Domain;
using SSAReplacement.Api.Features.Auth.Infrastructure;
using System.Security.Claims;

namespace SSAReplacement.Api.Features.Auth.Handlers;

public static class GetToken
{
    public static async Task<IResult> Handler(ClaimsPrincipal user, TokenService tokenService)
    {
        var username = user.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Results.Unauthorized();

        var accessToken = tokenService.GenerateAccessToken(username, out var expiresAt);
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(username);

        return Results.Ok(new TokenResponse(accessToken, refreshToken.Token, expiresAt));
    }
}

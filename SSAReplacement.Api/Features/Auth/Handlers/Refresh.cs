using SSAReplacement.Api.Features.Auth.Domain;
using SSAReplacement.Api.Features.Auth.Infrastructure;

namespace SSAReplacement.Api.Features.Auth.Handlers;

public static class Refresh
{
    public static async Task<IResult> Handler(RefreshRequest req, TokenService tokenService)
    {
        var existingToken = await tokenService.GetValidRefreshTokenAsync(req.RefreshToken);
        if (existingToken is null)
            return Results.Unauthorized();

        var accessToken = tokenService.GenerateAccessToken(existingToken.Username, existingToken.UserId, out var expiresAt);
        var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(existingToken.Username, existingToken.UserId);

        await tokenService.RevokeRefreshTokenAsync(existingToken, newRefreshToken.Token);

        return Results.Ok(new TokenResponse(accessToken, newRefreshToken.Token, expiresAt));
    }
}

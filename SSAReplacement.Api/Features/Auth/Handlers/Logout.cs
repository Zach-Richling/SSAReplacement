using SSAReplacement.Api.Features.Auth.Domain;
using SSAReplacement.Api.Features.Auth.Infrastructure;

namespace SSAReplacement.Api.Features.Auth.Handlers;

public static class Logout
{
    public static async Task<IResult> Handler(RefreshRequest req, TokenService tokenService)
    {
        var existingToken = await tokenService.GetValidRefreshTokenAsync(req.RefreshToken);
        if (existingToken is not null)
            await tokenService.RevokeRefreshTokenAsync(existingToken);

        return Results.Ok();
    }
}

using Microsoft.AspNetCore.Authentication.Negotiate;
using SSAReplacement.Api.Features.Auth.Handlers;

namespace SSAReplacement.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/token", GetToken.Handler)
            .RequireAuthorization(policy =>
                policy.AddAuthenticationSchemes(NegotiateDefaults.AuthenticationScheme)
                      .RequireAuthenticatedUser());

        group.MapPost("/refresh", Refresh.Handler)
            .AllowAnonymous();

        group.MapPost("/logout", Logout.Handler)
            .AllowAnonymous();

        group.MapGet("/photo", GetPhoto.Handler)
            .RequireAuthorization();
    }
}

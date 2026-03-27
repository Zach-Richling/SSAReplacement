using System.DirectoryServices;
using System.Security.Claims;

namespace SSAReplacement.Api.Features.Auth.Handlers;

public static class GetPhoto
{
    public static IResult Handler(ClaimsPrincipal user, ILogger<object> logger)
    {
        var username = user.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Results.Unauthorized();

        // Strip domain prefix (DOMAIN\user) or suffix (user@domain)
        var samAccountName = username.Contains('\\')
            ? username.Split('\\').Last()
            : username.Contains('@')
                ? username.Split('@').First()
                : username;

        try
        {
            using var searcher = new DirectorySearcher();
            searcher.Filter = $"(&(objectClass=user)(sAMAccountName={samAccountName}))";
            searcher.PropertiesToLoad.Add("thumbnailPhoto");

            var result = searcher.FindOne();
            if (result?.Properties["thumbnailPhoto"]?.Count > 0)
            {
                var photoBytes = (byte[])result.Properties["thumbnailPhoto"][0];
                return Results.File(photoBytes, "image/jpeg");
            }
        }
        catch (Exception ex)
        {
            // AD may not be available (local accounts, workgroup machines, etc.)
            logger.LogWarning(ex, "Could not retrieve thumbnail photo for user {Username}", samAccountName);
        }

        return Results.NotFound();
    }
}

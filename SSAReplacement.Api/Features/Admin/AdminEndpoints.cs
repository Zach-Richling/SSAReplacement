using SSAReplacement.Api.Features.Admin.Handlers;

namespace SSAReplacement.Api.Features.Admin;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin").WithTags("Admin").RequireAuthorization();

        group.MapGet("/users", GetUsers.Handler);
        group.MapGet("/users/{id:long}", GetUserById.Handler);
        group.MapGet("/audit", GetAuditEntries.Handler);
    }
}

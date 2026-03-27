using SSAReplacement.Api.Features.Dashboard.Handlers;

namespace SSAReplacement.Api.Features.Dashboard;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/dashboard").WithTags("Dashboard").RequireAuthorization();

        group.MapGet("/summary", GetDashboardSummary.Handler);
        group.MapGet("/upcoming-runs", GetUpcomingRuns.Handler);
        group.MapGet("/failure-spotlight", GetFailureSpotlight.Handler);
        group.MapGet("/run-history", GetRunHistory.Handler);
    }
}

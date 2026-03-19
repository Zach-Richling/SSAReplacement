using SSAReplacement.Api.Features.JobRuns.Handlers;

namespace SSAReplacement.Api.Features.JobRuns;

public static class JobRunEndpoints
{
    public static void MapJobRunEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/runs").WithTags("Job Runs");

        group.MapGet("/{id:int}", GetJobRunById.Handler);
        group.MapGet("/{id:int}/logs", GetJobRunLogs.Handler);
        group.MapGet("/{id:int}/logs/stream", StreamJobRunLogs.Handler);
    }
}

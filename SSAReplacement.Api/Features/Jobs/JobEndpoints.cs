using SSAReplacement.Api.Features.Jobs.Handlers;

namespace SSAReplacement.Api.Features.Jobs;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/jobs").WithTags("Jobs");

        group.MapGet("/", GetJobs.Handler);
        group.MapGet("/{id:long}", GetJobById.Handler);
        group.MapPost("/", CreateJob.Handler);
        group.MapPut("/{id:long}", UpdateJob.Handler);
        group.MapDelete("/{id:long}", DeleteJob.Handler);
        group.MapPut("/{id:long}/schedules", UpdateJobSchedules.Handler);
        group.MapGet("/{id:long}/parameters", GetJobParameters.Handler);
        group.MapPut("/{id:long}/parameters", UpdateJobParameters.Handler);
        group.MapPost("/{id:long}/trigger", TriggerJob.Handler);
        group.MapGet("/{id:long}/runs", GetJobRuns.Handler);
    }
}

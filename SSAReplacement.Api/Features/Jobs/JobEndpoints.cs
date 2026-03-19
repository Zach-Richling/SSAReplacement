using SSAReplacement.Api.Features.Jobs.Handlers;

namespace SSAReplacement.Api.Features.Jobs;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/jobs").WithTags("Jobs");

        group.MapGet("/", GetJobs.Handler);
        group.MapGet("/{id:int}", GetJobById.Handler);
        group.MapPost("/", CreateJob.Handler);
        group.MapPut("/{id:int}", UpdateJob.Handler);
        group.MapDelete("/{id:int}", DeleteJob.Handler);
        group.MapPut("/{id:int}/schedules", UpdateJobSchedules.Handler);
        group.MapGet("/{id:int}/parameters", GetJobParameters.Handler);
        group.MapPut("/{id:int}/parameters", UpdateJobParameters.Handler);
        group.MapPost("/{id:int}/trigger", TriggerJob.Handler);
        group.MapGet("/{id:int}/runs", GetJobRuns.Handler);
    }
}

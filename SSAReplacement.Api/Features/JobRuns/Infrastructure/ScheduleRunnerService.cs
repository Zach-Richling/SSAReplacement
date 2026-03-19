using Hangfire;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.JobRuns.Infrastructure;

public sealed class ScheduleRunnerService(
    IServiceScopeFactory scopeFactory,
    IBackgroundJobClient jobClient,
    ILogger<ScheduleRunnerService> logger)
{
    public async Task RunScheduleAsync(int scheduleId, CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var jobIds = await db.JobSchedules
            .Where(js => js.ScheduleId == scheduleId && js.Job.IsEnabled)
            .Select(js => js.JobId)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Schedule {ScheduleId} firing: enqueueing {Count} jobs", scheduleId, jobIds.Count);

        foreach (var jobId in jobIds)
            jobClient.Enqueue<JobRunnerService>(s => s.RunAsync(jobId, scheduleId));
    }
}

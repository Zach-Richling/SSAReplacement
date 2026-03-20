using Hangfire;
using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.JobRuns.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Schedules.Infrastructure;

public interface IScheduleHangfireSyncService
{
    Task SyncAllSchedulesAsync(CancellationToken cancellationToken = default);
    Task AddOrUpdateScheduleAsync(long scheduleId, string cronExpression, bool isEnabled, CancellationToken cancellationToken = default);
    Task RemoveScheduleAsync(long scheduleId, CancellationToken cancellationToken = default);
}

public sealed class ScheduleHangfireSyncService(
    IServiceScopeFactory scopeFactory,
    IRecurringJobManager jobManager) : IScheduleHangfireSyncService
{
    public async Task SyncAllSchedulesAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var schedules = await db.Schedules.AsNoTracking().ToListAsync(cancellationToken);
        foreach (var s in schedules)
        {
            if (s.IsEnabled)
                jobManager.AddOrUpdate<ScheduleRunnerService>($"schedule-{s.Id}", x => x.RunScheduleAsync(s.Id, CancellationToken.None), s.CronExpression, new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Local
                });
            else
                jobManager.RemoveIfExists($"schedule-{s.Id}");
        }
    }

    public Task AddOrUpdateScheduleAsync(long scheduleId, string cronExpression, bool isEnabled, CancellationToken cancellationToken = default)
    {
        if (isEnabled)
            jobManager.AddOrUpdate<ScheduleRunnerService>($"schedule-{scheduleId}", x => x.RunScheduleAsync(scheduleId, CancellationToken.None), cronExpression, new RecurringJobOptions()
            {
                TimeZone = TimeZoneInfo.Local
            });
        else
            jobManager.RemoveIfExists($"schedule-{scheduleId}");

        return Task.CompletedTask;
    }

    public Task RemoveScheduleAsync(long scheduleId, CancellationToken cancellationToken = default)
    {
        jobManager.RemoveIfExists($"schedule-{scheduleId}");
        return Task.CompletedTask;
    }
}

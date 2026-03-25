using Microsoft.EntityFrameworkCore;
using SSAReplacement.Api.Features.Dashboard.Domain;
using SSAReplacement.Api.Features.Schedules.Infrastructure;
using SSAReplacement.Api.Infrastructure;

namespace SSAReplacement.Api.Features.Dashboard.Handlers;

public static class GetDashboardSummary
{
    public static async Task<IResult> Handler(AppDbContext db, int rangeHours = 24)
    {
        var utcNow = DateTime.UtcNow;
        var cutoff = utcNow.AddHours(-rangeHours);
        var endOfDay = utcNow.Date.AddDays(1);

        var runningJobs = await db.JobRuns.CountAsync(r => r.Status == "Running");
        var failedRuns = await db.JobRuns.CountAsync(r => r.Status == "Failed" && r.StartedAt >= cutoff);
        var enabledJobs = await db.Jobs.CountAsync(j => j.IsEnabled);
        var disabledJobs = await db.Jobs.CountAsync(j => !j.IsEnabled);

        // Compute scheduled-today count from cron expressions
        var jobsWithSchedules = await db.Jobs
            .AsNoTracking()
            .Where(j => j.IsEnabled)
            .Include(j => j.JobSchedules)
                .ThenInclude(js => js.Schedule)
            .ToListAsync();

        var scheduledToday = 0;
        foreach (var job in jobsWithSchedules)
        {
            foreach (var js in job.JobSchedules)
            {
                if (js.Schedule is not { IsEnabled: true } schedule)
                    continue;

                var next = ScheduleHelpers.TryGetNextOccurrenceUtc(schedule.CronExpression, utcNow);
                if (next is DateTime dt && dt < endOfDay)
                {
                    scheduledToday++;
                    break;
                }
            }
        }

        return Results.Ok(new DashboardSummaryDto(runningJobs, failedRuns, enabledJobs, disabledJobs, scheduledToday));
    }
}

public static class GetUpcomingRuns
{
    public static async Task<IResult> Handler(AppDbContext db)
    {
        var utcNow = DateTime.UtcNow;

        var jobsWithSchedules = await db.Jobs
            .AsNoTracking()
            .Where(j => j.IsEnabled)
            .Include(j => j.JobSchedules)
                .ThenInclude(js => js.Schedule)
            .ToListAsync();

        var upcomingRuns = new List<UpcomingRunDto>();

        foreach (var job in jobsWithSchedules)
        {
            DateTime? earliest = null;
            foreach (var js in job.JobSchedules)
            {
                if (js.Schedule is not { IsEnabled: true } schedule)
                    continue;

                var next = ScheduleHelpers.TryGetNextOccurrenceUtc(schedule.CronExpression, utcNow);
                if (next is DateTime dt && (earliest is null || dt < earliest))
                    earliest = dt;
            }

            if (earliest is not null)
                upcomingRuns.Add(new UpcomingRunDto(job.Id, job.Name, earliest.Value));
        }

        return Results.Ok(upcomingRuns.OrderBy(r => r.NextRunUtc).Take(10).ToList());
    }
}

public static class GetFailureSpotlight
{
    public static async Task<IResult> Handler(AppDbContext db, int rangeHours = 24)
    {
        var cutoff = DateTime.UtcNow.AddHours(-rangeHours);

        var failureSpotlight = await db.JobRuns
            .AsNoTracking()
            .Where(r => r.Status == "Failed" && r.StartedAt >= cutoff)
            .GroupBy(r => r.JobId)
            .Select(g => new
            {
                JobId = g.Key,
                FailureCount = g.Count(),
                LastFailureUtc = g.Max(r => r.StartedAt)
            })
            .Where(g => g.FailureCount >= 2)
            .OrderByDescending(g => g.FailureCount)
            .Take(10)
            .ToListAsync();

        var failureJobIds = failureSpotlight.Select(f => f.JobId).ToList();
        var jobNames = await db.Jobs
            .AsNoTracking()
            .Where(j => failureJobIds.Contains(j.Id))
            .ToDictionaryAsync(j => j.Id, j => j.Name);

        var result = failureSpotlight
            .Select(f => new FailureSpotlightDto(
                f.JobId,
                jobNames.GetValueOrDefault(f.JobId, "Unknown"),
                f.FailureCount,
                f.LastFailureUtc))
            .ToList();

        return Results.Ok(result);
    }
}

public static class GetRunHistory
{
    public static async Task<IResult> Handler(AppDbContext db)
    {
        var utcNow = DateTime.UtcNow;
        var cutoff = utcNow.AddHours(-24);

        var runs = await db.JobRuns
            .AsNoTracking()
            .Where(r => r.StartedAt >= cutoff && (r.Status == "Success" || r.Status == "Failed"))
            .Select(r => new { r.StartedAt, r.Status })
            .ToListAsync();

        // Build 24 hourly buckets
        var result = new List<RunHistoryBucketDto>();
        for (var i = 23; i >= 0; i--)
        {
            var bucketStart = utcNow.AddHours(-i).Date.AddHours(utcNow.AddHours(-i).Hour);
            var bucketEnd = bucketStart.AddHours(1);

            var bucketRuns = runs.Where(r => r.StartedAt >= bucketStart && r.StartedAt < bucketEnd);
            result.Add(new RunHistoryBucketDto(
                bucketStart.ToLocalTime().ToString("h tt").ToLower(),
                bucketRuns.Count(r => r.Status == "Success"),
                bucketRuns.Count(r => r.Status == "Failed")));
        }

        return Results.Ok(result);
    }
}

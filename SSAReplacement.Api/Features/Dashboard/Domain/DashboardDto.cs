namespace SSAReplacement.Api.Features.Dashboard.Domain;

public record DashboardSummaryDto(
    int RunningJobs,
    int FailedRuns,
    int EnabledJobs,
    int DisabledJobs,
    int ScheduledRunsToday);

public record UpcomingRunDto(
    long JobId,
    string JobName,
    DateTime NextRunUtc);

public record FailureSpotlightDto(
    long JobId,
    string JobName,
    int FailureCount,
    DateTime? LastFailureUtc);

public record RunHistoryBucketDto(
    string Hour,
    int Success,
    int Failed);

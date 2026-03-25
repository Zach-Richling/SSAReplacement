namespace SSAReplacement.Wasm.Client.Dashboard;

public record DashboardSummary(
    int RunningJobs,
    int FailedRuns,
    int EnabledJobs,
    int DisabledJobs,
    int ScheduledRunsToday);

public record UpcomingRun(long JobId, string JobName, DateTime NextRunUtc);

public record FailureSpotlight(long JobId, string JobName, int FailureCount, DateTime? LastFailureUtc);

public record RunHistoryBucket(string Hour, int Success, int Failed);

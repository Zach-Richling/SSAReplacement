using System.Net.Http.Json;

namespace SSAReplacement.Wasm.Client.Dashboard;

public class DashboardEndpoints(HttpClient http)
{
    public async Task<DashboardSummary?> GetSummaryAsync(int rangeHours = 24, CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<DashboardSummary>($"dashboard/summary?rangeHours={rangeHours}", cancellationToken);
    }

    public async Task<List<UpcomingRun>> GetUpcomingRunsAsync(CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<List<UpcomingRun>>("dashboard/upcoming-runs", cancellationToken) ?? [];
    }

    public async Task<List<FailureSpotlight>> GetFailureSpotlightAsync(int rangeHours = 24, CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<List<FailureSpotlight>>($"dashboard/failure-spotlight?rangeHours={rangeHours}", cancellationToken) ?? [];
    }

    public async Task<List<RunHistoryBucket>> GetRunHistoryAsync(CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<List<RunHistoryBucket>>("dashboard/run-history", cancellationToken) ?? [];
    }
}

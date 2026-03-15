using System.Net.Http.Json;

namespace SSAReplacement.Wasm.Client.Schedules;

public class ScheduleEndpoints(HttpClient http)
{
    /// <summary>
    /// GET /schedules. Returns the list of schedules.
    /// </summary>
    public async Task<List<Schedule>> GetSchedulesAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<Schedule>>("schedules", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// POST /schedules. Creates a schedule. Returns the created schedule on success.
    /// Throws <see cref="HttpRequestException"/> with the response body message on 400 (e.g. invalid cron).
    /// </summary>
    public async Task<Schedule> AddScheduleAsync(CreateScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("schedules", request, cancellationToken);

        res.EnsureSuccessStatusCode();

        return await res.Content.ReadFromJsonAsync<Schedule>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");
    }

    public async Task UpdateScheduleAsync(int scheduleId, UpdateScheduleRequest updateRequest, CancellationToken cancellationToken = default)
    {
        var res = await http.PutAsJsonAsync($"schedules/{scheduleId}", updateRequest, cancellationToken);
        res.EnsureSuccessStatusCode();
    }
}

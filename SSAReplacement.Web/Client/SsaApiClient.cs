using System.Net;
using System.Net.Http.Json;

namespace SSAReplacement.Web.Client;

/// <summary>
/// Typed HTTP client for the SSA Replacement API. Lives in the Web project; no reference to the Api project.
/// </summary>
public class SsaApiClient(HttpClient http)
{

    /// <summary>
    /// GET /jobs. Returns the list of jobs.
    /// </summary>
    public async Task<List<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<Job>>("jobs", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// GET /executables. Returns the list of executables.
    /// </summary>
    public async Task<List<Executable>> GetExecutablesAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<Executable>>("executables", cancellationToken);
        return list ?? [];
    }

    /// <summary>
    /// POST /jobs. Creates a job. Returns the created job on success.
    /// Throws <see cref="HttpRequestException"/> with message on 404 (executable not found) or other error.
    /// </summary>
    public async Task<AddJobResponse> AddJobAsync(AddJobRequest request, CancellationToken cancellationToken = default)
    {
        var res = await http.PostAsJsonAsync("jobs", request, cancellationToken);

        if (res.StatusCode == HttpStatusCode.NotFound)
            throw new HttpRequestException("Executable not found.");

        res.EnsureSuccessStatusCode();

        var job = await res.Content.ReadFromJsonAsync<AddJobResponse>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");

        return job;
    }

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

        if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            var message = await res.Content.ReadFromJsonAsync<string>(cancellationToken)
                ?? await res.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(string.IsNullOrWhiteSpace(message) ? "Invalid request." : message);
        }

        res.EnsureSuccessStatusCode();

        var schedule = await res.Content.ReadFromJsonAsync<Schedule>(cancellationToken)
            ?? throw new HttpRequestException("Unexpected empty response from server.");

        return schedule;
    }
}

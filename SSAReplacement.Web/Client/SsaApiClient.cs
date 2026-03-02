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
}

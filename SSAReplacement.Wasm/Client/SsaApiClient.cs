using SSAReplacement.Wasm.Client.Executables;
using SSAReplacement.Wasm.Client.Jobs;
using SSAReplacement.Wasm.Client.Schedules;

namespace SSAReplacement.Wasm.Client;

/// <summary>
/// Typed HTTP client for the SSA Replacement API.
/// </summary>
public class SsaApiClient(HttpClient http)
{
    public readonly JobEndpoints Job = new(http);
    public readonly ExecutableEndpoints Executable = new(http);
    public readonly ScheduleEndpoints Schedule = new(http);
}

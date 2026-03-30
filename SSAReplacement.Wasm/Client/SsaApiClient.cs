using SSAReplacement.Wasm.Client.Admin;
using SSAReplacement.Wasm.Client.Auth;
using SSAReplacement.Wasm.Client.Dashboard;
using SSAReplacement.Wasm.Client.Executables;
using SSAReplacement.Wasm.Client.Jobs;
using SSAReplacement.Wasm.Client.Schedules;

namespace SSAReplacement.Wasm.Client;

/// <summary>
/// Typed HTTP client for the SSA Replacement API.
/// </summary>
public class SsaApiClient(HttpClient http)
{
    public readonly AuthEndpoints Auth = new(http);
    public readonly DashboardEndpoints Dashboard = new(http);
    public readonly JobEndpoints Job = new(http);
    public readonly ExecutableEndpoints Executable = new(http);
    public readonly ScheduleEndpoints Schedule = new(http);
    public readonly AdminEndpoints Admin = new(http);
}

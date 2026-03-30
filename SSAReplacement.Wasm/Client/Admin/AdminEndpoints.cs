using System.Net.Http.Json;

namespace SSAReplacement.Wasm.Client.Admin;

public class AdminEndpoints(HttpClient http)
{
    public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var list = await http.GetFromJsonAsync<List<User>>("admin/users", cancellationToken);
        return list ?? [];
    }

    public async Task<UserDetail?> GetUserAsync(long id, CancellationToken cancellationToken = default)
    {
        return await http.GetFromJsonAsync<UserDetail>($"admin/users/{id}", cancellationToken);
    }

    public async Task<List<AuditEntry>> GetAuditEntriesAsync(
        string? entityName = null,
        long? userId = null,
        string? action = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>();
        if (entityName is not null) qs.Add($"entityName={Uri.EscapeDataString(entityName)}");
        if (userId is not null) qs.Add($"userId={userId}");
        if (action is not null) qs.Add($"action={Uri.EscapeDataString(action)}");

        var url = qs.Count > 0 ? $"admin/audit?{string.Join("&", qs)}" : "admin/audit";
        var list = await http.GetFromJsonAsync<List<AuditEntry>>(url, cancellationToken);
        return list ?? [];
    }
}

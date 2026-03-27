using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace SSAReplacement.Wasm.Client.Auth;

public class AuthEndpoints(HttpClient http)
{
    public async Task<TokenResponse?> GetTokenAsync(CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/token");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await http.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenResponse>(ct);
    }

    public async Task<TokenResponse?> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("auth/refresh", new RefreshRequest(refreshToken), ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenResponse>(ct);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await http.PostAsJsonAsync("auth/logout", new RefreshRequest(refreshToken), ct);
    }

    public async Task<string?> GetPhotoDataUrlAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync("auth/photo", ct);
        if (!response.IsSuccessStatusCode)
            return null;

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        return $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
    }
}

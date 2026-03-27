using SSAReplacement.Wasm.Client.Auth;
using System.Net;
using System.Net.Http.Headers;

namespace SSAReplacement.Wasm.Auth;

public class AuthTokenHandler(TokenStorageService tokenStorage, IServiceProvider serviceProvider) : DelegatingHandler
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Don't attach tokens to auth endpoints (except logout)
        var path = request.RequestUri?.PathAndQuery ?? "";
        if (path.Contains("/auth/token") || path.Contains("/auth/refresh"))
            return await base.SendAsync(request, cancellationToken);

        var accessToken = await tokenStorage.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(accessToken))
        {
            var newToken = await TryRefreshTokenAsync(cancellationToken);
            if (newToken is not null)
            {
                // Clone and retry the request with the new token
                var retry = await CloneRequestAsync(request);
                retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response.Dispose();
                response = await base.SendAsync(retry, cancellationToken);
            }
        }

        return response;
    }

    private async Task<string?> TryRefreshTokenAsync(CancellationToken ct)
    {
        await _refreshLock.WaitAsync(ct);
        try
        {
            var refreshToken = await tokenStorage.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            // Use a separate HttpClient to avoid infinite recursion
            using var scope = serviceProvider.CreateScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("AuthRefresh");
            var authClient = new AuthEndpoints(httpClient);

            var result = await authClient.RefreshAsync(refreshToken, ct);
            if (result is null)
            {
                await tokenStorage.ClearTokensAsync();
                return null;
            }

            await tokenStorage.SetTokensAsync(result.AccessToken, result.RefreshToken);
            return result.AccessToken;
        }
        catch
        {
            await tokenStorage.ClearTokensAsync();
            return null;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);
        if (request.Content is not null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);
            if (request.Content.Headers.ContentType is not null)
                clone.Content.Headers.ContentType = request.Content.Headers.ContentType;
        }

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}

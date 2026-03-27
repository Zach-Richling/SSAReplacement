using Microsoft.JSInterop;

namespace SSAReplacement.Wasm.Auth;

public class TokenStorageService(IJSRuntime jsRuntime)
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    private IJSObjectReference? _module;

    private async Task<IJSObjectReference> GetModuleAsync()
    {
        _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/auth-storage.js");
        return _module;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<string?>("getItem", AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<string?>("getItem", RefreshTokenKey);
    }

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("setItem", AccessTokenKey, accessToken);
        await module.InvokeVoidAsync("setItem", RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("removeItem", AccessTokenKey);
        await module.InvokeVoidAsync("removeItem", RefreshTokenKey);
    }
}

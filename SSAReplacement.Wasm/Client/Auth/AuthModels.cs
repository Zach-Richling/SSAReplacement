namespace SSAReplacement.Wasm.Client.Auth;

public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public record RefreshRequest(string RefreshToken);

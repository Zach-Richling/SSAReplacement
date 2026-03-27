namespace SSAReplacement.Api.Features.Auth.Domain;

public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public record RefreshRequest(string RefreshToken);

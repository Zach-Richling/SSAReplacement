namespace SSAReplacement.Api.Features.Auth.Infrastructure;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string Secret { get; init; }
    public string Issuer { get; init; } = "SSAReplacement";
    public string Audience { get; init; } = "SSAReplacement";
    public int ExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}

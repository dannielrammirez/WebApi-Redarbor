namespace Redarbor.Infrastructure.Authentication;

public sealed class TokenSettings
{
    public const string SectionName = "TokenSettings";

    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = "Redarbor.API";
    public string Audience { get; init; } = "Redarbor.Client";
    public int ExpirationInMinutes { get; init; } = 60;
}

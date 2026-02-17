namespace SnowrunnerMerger.Api.Models.Auth;

public class AuthCode
{
    public string Code { get; init; }
    public Guid UserId { get; init; }
    public User User { get; init; }
    public string CodeChallenge { get; init; }
    public DateTime ExpiresAt { get; init; }
}
namespace SnowrunnerMerger.Api.Models.Auth.OAuth;

public class OAuthConnection
{
    public string Provider { get; set; }
    public string ProviderAccountId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
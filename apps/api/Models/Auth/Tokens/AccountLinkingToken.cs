using System.Text.Json.Serialization;

namespace SnowrunnerMerger.Api.Models.Auth.Tokens;

public class AccountLinkingToken : UserToken
{
    [JsonIgnore]
    public string Provider { get; set; }
    [JsonIgnore]
    public string ProviderAccountId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
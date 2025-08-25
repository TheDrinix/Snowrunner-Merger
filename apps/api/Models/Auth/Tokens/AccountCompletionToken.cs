using System.Text.Json.Serialization;

namespace SnowrunnerMerger.Api.Models.Auth.Tokens;

public class AccountCompletionToken : UserToken
{
    [JsonIgnore]
    public string Provider { get; set; }
    [JsonIgnore]
    public string ProviderAccountId { get; set; }
    public string Email { get; set; }
}
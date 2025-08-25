using System.Text.Json.Serialization;

namespace SnowrunnerMerger.Api.Models.Auth.Tokens;

public class AccountCompletionToken : UserToken
{
    [JsonIgnore]
    public string GoogleId { get; set; }
    public string Email { get; set; }
}
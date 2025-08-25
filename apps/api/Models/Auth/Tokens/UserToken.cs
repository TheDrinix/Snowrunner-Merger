using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SnowrunnerMerger.Api.Models.Auth.Tokens;

[Index(nameof(Token), IsUnique = true)]
public class UserToken
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}
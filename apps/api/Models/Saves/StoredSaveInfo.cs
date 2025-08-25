
using System.Text.Json.Serialization;

namespace SnowrunnerMerger.Api.Models.Saves;

public class StoredSaveInfo
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int SaveNumber { get; set; }
    public DateTime UploadedAt { get; set; }
    public Guid SaveGroupId { get; set; }
    [JsonIgnore]
    public SaveGroup SaveGroup { get; set; }
}
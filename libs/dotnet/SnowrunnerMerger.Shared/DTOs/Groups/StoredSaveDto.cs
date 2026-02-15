using SnowrunnerMerger.Shared.Models;

namespace SnowrunnerMerger.Shared.DTOs.Groups;

public class StoredSaveDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SaveNumber { get; set; }
    public DateTime UploadedAt { get; set; }
    public List<Map> DiscoveredMaps { get; set; } = [];
    public Guid SaveGroupId { get; set; }
}
using System.Text.Json;

namespace SnowrunnerMerger.Shared.Models;

public class Save(
    Dictionary<string, dynamic> rawSaveData,
    int saveNumber)
{
    public Dictionary<string, dynamic> RawSaveData { get; set; } = rawSaveData;
    public string SaveDataKey { get; set; } = rawSaveData.Keys.First();
    public int SaveNumber { get; set; } = saveNumber;
    public SaveData? SaveData => JsonSerializer.Deserialize<SaveData>(RawSaveData[SaveDataKey]);
}

using SnowrunnerMerger.Api.Models.Saves;

namespace SnowrunnerMerger.Api.Services.Interfaces;

public abstract class IStorageService
{
    protected static readonly string StorageDir = Path.Join("storage", "saves");
    protected static readonly string TmpStorageDir = Path.Join(StorageDir, "tmp");
    public static readonly int MaxSaveSize = 50 * 1024 * 1024;
    
    public IStorageService()
    {
        InitializeStorage();
    }

    public abstract Task StoreGroupSave(Guid saveId, IFormFile file, int saveSlot);
    public abstract Task StoreTmpSave(Guid userId, IFormFile file, int saveSlot);
    public abstract Save? ReadTmpSave(Guid userId, int saveSlot);
    public abstract Save? ReadGroupSave(Guid saveId, int saveSlot);
    public abstract void RemoveTmpStorage(Guid userId);
    public abstract void RemoveGroupSave(StoredSaveInfo save);
    public abstract void CopyTmpSaveMapDataToOutput(Guid userId, int sourceSaveSlot, int outputSaveSlot);
    public abstract void CopyGroupSaveMapDataToOutput(Guid userId, StoredSaveInfo storedSaveInfo, int outputSaveSlot, string type, string[] maps);
    public abstract void StoreOutputSaveData(Guid userId, Dictionary<string, dynamic> saveData, int outputSaveSlot);
    public abstract MemoryStream ZipOutputSaveDataStream(Guid userId);

    protected abstract void InitializeStorage();
}
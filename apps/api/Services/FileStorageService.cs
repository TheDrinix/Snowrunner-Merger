using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using SnowrunnerMerger.Api.Models.Saves;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services;

public class FileStorageService : IStorageService
{
    public override Task StoreGroupSave(Guid saveId, IFormFile file, int saveSlot)
    {
        var id = saveId.ToString();
        var saveDir = Path.Join(StorageDir, id);
        return StoreSave(id, saveDir, file, saveSlot);
    }
    
    public override Task StoreTmpSave(Guid userId, IFormFile file, int saveSlot)
    {
        var id = userId.ToString();
        var saveDir = Path.Join(TmpStorageDir, id, "save");
        return StoreSave(id, saveDir, file, saveSlot);
    }
    
    public override Save? ReadTmpSave(Guid userId, int saveSlot)
    {
        var id = userId.ToString();
        var saveDir = Path.Join(TmpStorageDir, id, "save");
        
        return ReadSave(saveDir, saveSlot);
    }

    public override Save? ReadGroupSave(Guid saveId, int saveSlot)
    {
        var id = saveId.ToString();
        var saveDir = Path.Join(StorageDir, id);

        return ReadSave(saveDir, saveSlot);
    }

    public override void RemoveTmpStorage(Guid userId)
    {
        var path = Path.Join(TmpStorageDir, userId.ToString());
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public override void RemoveGroupSave(StoredSaveInfo save)
    {
        var path = Path.Join(StorageDir, save.Id.ToString());
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public override void CopyTmpSaveMapDataToOutput(Guid userId, int sourceSaveSlot, int outputSaveSlot)
    {
        var sourceDir = Path.Join(TmpStorageDir, userId.ToString(), "save");
        var destDir = Path.Join(TmpStorageDir, userId.ToString(), "output");
        
        CopySaveMapDataToOutput(sourceDir, destDir, sourceSaveSlot, outputSaveSlot);
    }

    public override void CopyGroupSaveMapDataToOutput(Guid userId, StoredSaveInfo storedSaveInfo, int outputSaveSlot)
    {
        var sourceDir = Path.Join(StorageDir, storedSaveInfo.Id.ToString());
        var destDir = Path.Join(StorageDir, userId.ToString(), "output");
        
        CopySaveMapDataToOutput(sourceDir, destDir, storedSaveInfo.SaveNumber, outputSaveSlot);
    }

    public override void StoreOutputSaveData(Guid userId, Dictionary<string, dynamic> saveData, int outputSaveSlot)
    {
        var outputSaveJson = JsonSerializer.Serialize(saveData);
        
        var outputSaveName = $"CompleteSave{(outputSaveSlot > 0 ? outputSaveSlot.ToString() : "")}.dat";
        var outputSavePath = Path.Join(TmpStorageDir, userId.ToString(), "output", outputSaveName);
        
        File.WriteAllText(outputSavePath, outputSaveJson);
    }

    public override MemoryStream ZipOutputSaveDataStream(Guid userId)
    {
        var outputDir = Path.Join(TmpStorageDir, userId.ToString(), "output");
        
        // Put zip into a memory stream
        var memoryStream = new MemoryStream();
        ZipFile.CreateFromDirectory(outputDir, memoryStream);
        
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        RemoveTmpStorage(userId);
        
        return memoryStream;
    }
    
    protected override void InitializeStorage()
    {
        if (!Directory.Exists(StorageDir))
        {
            Directory.CreateDirectory(StorageDir);
        }
        
        // Clear tmp storage on initialization
        if (Directory.Exists(TmpStorageDir))
        {
            Directory.Delete(TmpStorageDir, true);
        }
        Directory.CreateDirectory(TmpStorageDir);
    }

    private async Task StoreSave(string id, string saveDir, IFormFile file, int saveSlot) 
    {
        if (Directory.Exists(saveDir))
        {
            Directory.Delete(saveDir, true);
        }
        Directory.CreateDirectory(saveDir);
        
        var zipPath = Path.Join(TmpStorageDir, $"{id}.zip");
        
        await using var stream = new FileStream(zipPath, FileMode.Create);
        await file.CopyToAsync(stream);
        stream.Close();
        
        var zipFile = ZipFile.OpenRead(zipPath);
        
        var declaredSize = zipFile.Entries.Sum(x => x.Length);
        if (declaredSize > MaxSaveSize)
        {
            Directory.Delete(saveDir, true);
            throw new InvalidDataException("Save file is too large.");
        }
        
        ZipFile.ExtractToDirectory(zipPath, saveDir);
        zipFile.Dispose();
        
        File.Delete(zipPath);
        
        if (!ValidateSaveContents(saveDir, saveSlot))
        {
            Directory.Delete(saveDir, true);
            throw new InvalidDataException("Save file is missing required data.");
        }
    }
    
    /// <summary>
    ///     Loads a save from a directory.
    /// </summary>
    /// <param name="dir">The directory containing the save file.</param>
    /// <param name="saveSlot">The save file number.</param>
    /// <returns>A <see cref="Save"/> object containing the save data.</returns>
    private Save? ReadSave(string dir, int saveSlot)
    {
        var savePath = Path.Join(dir, $"CompleteSave{(saveSlot > 0 ? saveSlot.ToString() : "")}.dat");

        if (!File.Exists(savePath)) return null;
        
        var saveFile = File.ReadAllText(savePath);
        if (saveFile[^1] != '}')
        {
            saveFile = saveFile.Remove(saveFile.Length - 1, 1);
        }
        
        var saveData = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(saveFile);
        
        return saveData is null ? null : new Save(saveData, saveSlot);
    }

    /// <summary>
    ///     Validates the save files in a directory.
    /// </summary>
    /// <param name="path">The path to the directory containing the save files.</param>
    /// <param name="saveSlot">The save number to validate.</param>
    /// <returns>True if the save files are valid, false otherwise.</returns>
    private bool ValidateSaveContents(string path, int saveSlot)
    {
        var saveFileName = $"CompleteSave{(saveSlot > 0 ? saveSlot : "")}.dat";
        var files = Directory
            .GetFiles(path)
            .Select(Path.GetFileName)
            .ToList();
        
        if (!files.Contains(saveFileName))
        {
            return false;
        }
        
        var mapDataRegex = new Regex($@"\b({(saveSlot > 0 ? saveSlot.ToString() + '_' : "")}(fog|sts)_.*\.dat)\b", RegexOptions.Multiline);
        var count = files.Count(file => mapDataRegex.IsMatch(Path.GetFileName(file)!));

        return count >= 2;
    }

    /// <summary>
    ///     Copies map data (sts and fog) files from a source directory to a destination directory,
    /// </summary>
    /// <param name="sourceDir">The source directory containing the map data files.</param>
    /// <param name="destDir">The destination directory where the map data files will be copied to.</param>
    /// <param name="sourceSaveSlot">The save slot number of the source files.</param>
    /// <param name="outputSaveSlot">The save slot number for the output files.</param>
    private void CopySaveMapDataToOutput(string sourceDir, string destDir, int sourceSaveSlot, int outputSaveSlot)
    {
        var mapDataFilesRegex = new Regex($@"\b({(sourceSaveSlot > 0 ? sourceSaveSlot.ToString() + '_' : "")}(fog|sts)_.*\.dat)\b", RegexOptions.Multiline);
        var mapDataFiles = Directory
            .GetFiles(sourceDir)
            .Where(f => mapDataFilesRegex.IsMatch(Path.GetFileName(f)));

        foreach (var file in mapDataFiles)
        {
            var sourceFileName = Path.GetFileName(file);
            if (sourceSaveSlot > 0)
            {
                sourceFileName = sourceFileName[2..];
            }
            
            var outputPrefix = outputSaveSlot > 0 ? outputSaveSlot.ToString() + '_' : "";
            
            var destFileName = outputPrefix + sourceFileName;
            var destFilePath = Path.Join(destDir, destFileName);
            
            File.Copy(file, destFilePath, true);
        }
    }
}
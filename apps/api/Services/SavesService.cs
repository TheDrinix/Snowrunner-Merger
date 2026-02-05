using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using SnowrunnerMerger.Api.Data;
using SnowrunnerMerger.Api.Exceptions;
using SnowrunnerMerger.Api.Models.Saves;
using SnowrunnerMerger.Api.Models.Saves.Dtos;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services;

public partial class SavesService : ISavesService
{
    private readonly ILogger<SavesService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IStorageService _storageService;
    
    public SavesService(
        ILogger<SavesService> logger,
        AppDbContext dbContext,
        IUserService userService,
        IStorageService storageService
    )
    {
        _logger = logger;
        _dbContext = dbContext;
        _userService = userService;
        _storageService = storageService;
    }
    
    /// <summary>
    /// Stores a save file for a specified group.
    /// </summary>
    /// <param name="groupId">The ID of the group to store the save file for.</param>
    /// <param name="data">An <see cref="UploadSaveDto"/> object containing the save file and its metadata.</param>
    /// <param name="saveSlot">The slot number to store the save file in.</param>
    /// <returns>A <see cref="StoredSaveInfo"/> object containing information about the stored save file.</returns>
    /// <exception cref="HttpResponseException">
    /// Thrown with different HTTP status codes depending on the validation failure:
    /// <list type="bullet">
    ///     <item>
    ///         HttpStatusCode.BadRequest (400): If the file type is invalid or the save file is too big.
    ///     </item>
    ///     <item>
    ///         HttpStatusCode.NotFound (404): If the group is not found.
    ///     </item>
    ///     <item>
    ///         HttpStatusCode.Unauthorized (401): If the user is not authorized to store the save file.
    ///     </item>
    /// </list>
    /// </exception>
    public async Task<StoredSaveInfo> StoreSave(Guid groupId, UploadSaveDto data, int saveSlot)
    {
        if (data.Save.ContentType != "application/zip") throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid file type");
        
        var sessionData = _userService.GetUserSessionData();
        
        var group = _dbContext.SaveGroups
            .Include(g => g.StoredSaves)
            .FirstOrDefault(g => g.Id == groupId);
        
        if (group is null) throw new HttpResponseException(HttpStatusCode.NotFound, "Group not found");

        if (!group.OwnerId.Equals(sessionData.Id)) throw new HttpResponseException(HttpStatusCode.Unauthorized);
        
        StoredSaveInfo? oldestSave = null; 
        if (group.StoredSaves.Count >= 3)
        {
            if ( saveSlot < 0 || saveSlot >= group.StoredSaves.Count)
            {
                oldestSave = group.StoredSaves.OrderBy(s => s.UploadedAt).First();
            }
            else
            {
                oldestSave = group.StoredSaves.OrderByDescending(s => s.UploadedAt).ElementAt(saveSlot);
            }
        }

        var saveInfo = new StoredSaveInfo
        {
            Description = data.Description,
            SaveNumber = data.SaveNumber,
            UploadedAt = DateTime.UtcNow,
            SaveGroupId = group.Id
        };
        
        _dbContext.StoredSaves.Add(saveInfo);

        try
        {
            await _storageService.StoreGroupSave(saveInfo.Id, data.Save, data.SaveNumber);
        } catch (InvalidDataException ex)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, ex.Message);
        }

        var storedSave = _storageService.ReadGroupSave(saveInfo.Id, data.SaveNumber);
        if (storedSave is not null && storedSave.SaveData is not null)
        {
            var knownMapsIds = new HashSet<string>();
            foreach (var visitedLevel in storedSave.SaveData.SslValue.visitedLevels)
            {
                // Known region format is "level_<region_tag (ru/us)>_<map_id>_<region_number>" e.g. "region_us_01_01"
                // We need to extract the <region_tag>_<map_id> part
                var match = KnownRegionRegex().Match(visitedLevel);
                if (!match.Success) continue;
                
                var mapId = $"{match.Groups[1].Value}_{match.Groups[2].Value}".ToUpper();
                knownMapsIds.Add(mapId);
            }
            
            var discoveredMaps = _dbContext.Maps
                .Where(m => knownMapsIds.Contains(m.Id))
                .ToList();

            saveInfo.DiscoveredMaps = discoveredMaps;
        }

        await _dbContext.SaveChangesAsync();

        if (oldestSave is not null)
        {
            await RemoveSave(oldestSave);
        }

        return saveInfo;
    }
    
    /// <summary>
    ///     Merges two save files from a group and returns the path to the merged save file.
    /// </summary>
    /// <param name="groupId">The ID of the group to merge the save files for.</param>
    /// <param name="data">A <see cref="MergeSavesDto"/> object containing the save file to merge and its metadata.</param>
    /// <param name="storedSaveNumber">The slot number of the stored save file to merge with.</param>
    /// <returns>A MemoryStream containing the merged save file.</returns>
    /// <exception cref="HttpResponseException">
    ///     Thrown with different HTTP status codes depending on the validation failure:
    ///     <list type="bullet">
    ///         <item>
    ///             HttpStatusCode.BadRequest (400): If the file type is invalid, the save file is too big, the save number is invalid, or the save is invalid.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.NotFound (404): If the group is not found.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.Unauthorized (401): If the user is not authorized to merge the save files.
    ///         </item>
    ///     </list>
    /// </exception>
    public async Task<MemoryStream> MergeSaves(Guid groupId, MergeSavesDto data, int storedSaveNumber)
    {
        if (data.Save.ContentType != "application/zip") throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid file type");
        
        var sessionData = _userService.GetUserSessionData();
        
        var group = _dbContext.SaveGroups
            .Include(g => g.Members)
            .FirstOrDefault(g => g.Id == groupId);
        
        if (group is null) throw new HttpResponseException(HttpStatusCode.NotFound, "Group not found");
        
        if (group.Members.All(m => !m.Id.Equals(sessionData.Id))) throw new HttpResponseException(HttpStatusCode.Unauthorized);
        
        var saves = _dbContext.StoredSaves
            .Where(s => s.SaveGroupId == groupId)
            .OrderByDescending(s => s.UploadedAt)
            .ToList();
        
        if (saves.Count == 0) throw new HttpResponseException(HttpStatusCode.BadRequest, "No group saves found");
        
        if (saves.Count < storedSaveNumber) storedSaveNumber = saves.Count - 1;
        
        if (storedSaveNumber < 0) throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid save number");

        try
        {
            await _storageService.StoreTmpSave(sessionData.Id, data.Save, data.SaveNumber);   
        } 
        catch (InvalidDataException ex)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, ex.Message);
        }

        var storedSaveData = saves[storedSaveNumber];

        _storageService.CopyTmpSaveMapDataToOutput(sessionData.Id, data.SaveNumber, data.OutputSaveNumber);
        
;
        var uploadedSave = _storageService.ReadTmpSave(sessionData.Id, data.SaveNumber);
        var storedSave = _storageService.ReadGroupSave(storedSaveData.Id, storedSaveData.SaveNumber);
        
        if (uploadedSave is null || storedSave is null)
        {
            _storageService.RemoveTmpStorage(sessionData.Id);
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Save is invalid");
        }
        
        var outputSaveData = MergeSaveData(uploadedSave, storedSave, data.OutputSaveNumber, data.Options, data.MergedMaps);
        
        if (outputSaveData is null)
        {
            _storageService.RemoveTmpStorage(sessionData.Id);
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Save is invalid");
        }

        var outputSave = new Dictionary<string, dynamic>()
        {
            { $"CompleteSave{(data.OutputSaveNumber > 0 ? data.OutputSaveNumber.ToString() : "")}", outputSaveData },
            { "cfg_version", uploadedSave.RawSaveData["cfg_version"] }
        };
        
        _storageService.StoreOutputSaveData(sessionData.Id, outputSave, data.OutputSaveNumber);
        if (data.Options.HasFlag(MergeOptions.MapProgress))
        {
            _storageService.CopyGroupSaveMapDataToOutput(sessionData.Id, storedSaveData, data.OutputSaveNumber, "fog", data.MergedMaps);
        }
        if (data.Options.HasFlag(MergeOptions.VehiclesInWorld))
        {
            _storageService.CopyGroupSaveMapDataToOutput(sessionData.Id, storedSaveData, data.OutputSaveNumber, "sts", data.MergedMaps);
        }

        return _storageService.ZipOutputSaveDataStream(sessionData.Id);
    }

    /// <summary>
    ///     Removes a save file from a group.
    /// </summary>
    /// <param name="saveId">The ID of the save file to remove.</param>
    /// <exception cref="HttpResponseException">
    ///     Thrown with different HTTP status codes depending on the validation failure:
    ///     <list type="bullet">
    ///         <item>
    ///             HttpStatusCode.NotFound (404): If the save file is not found.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.Unauthorized (401): If the user is not authorized to remove the save file.
    ///         </item>
    ///     </list>
    /// </exception>
    public Task RemoveSave(Guid saveId)
    {
        var sessionData = _userService.GetUserSessionData();
        
        var save = _dbContext.StoredSaves
            .Include(s => s.SaveGroup)
            .FirstOrDefault(s => s.Id == saveId);
        
        if (save is null) throw new HttpResponseException(HttpStatusCode.NotFound, "Save not found");
        
        if (!save.SaveGroup.OwnerId.Equals(sessionData.Id)) throw new HttpResponseException(HttpStatusCode.Unauthorized);

        return RemoveSave(save);
    }

    /// <summary>
    ///     Removes stored save data from the database and the file system.
    /// </summary>
    /// <param name="save">The <see cref="StoredSaveInfo"/> object to remove.</param>
    public Task RemoveSave(StoredSaveInfo save)
    {
        _storageService.RemoveGroupSave(save);
        
        _dbContext.StoredSaves.Remove(save);
        
        return _dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    ///     Merges two save files
    /// </summary>
    /// <param name="uploadedSave">A <see cref="Save"/> to merge with stored save.</param>
    /// <param name="storedSave">A <see cref="Save"/> to merge with uploaded save.</param>
    /// <param name="outputSaveNumber">The save slot number of the output save.</param>
    /// <param name="opt">The <see cref="MergeOptions"/> flags indicating which parts of the save to merge.</param>
    /// <param name="mergedMaps">An array of map IDs to merge. If empty, all maps will be merged.</param>
    /// <returns>A <see cref="SaveData"/> object containing the merged save data.</returns>
    private SaveData? MergeSaveData(Save uploadedSave, Save storedSave, int outputSaveNumber, MergeOptions opt, string[] mergedMaps)
    {
        if (uploadedSave.SaveData is null || storedSave.SaveData is null) return null;
        
        var outputSaveData = mergedMaps.Length > 0 ? 
            MergeSaveDataWithSpecifiedMaps(uploadedSave, storedSave, outputSaveNumber, opt, mergedMaps) : 
            MergeSaveDataAllMaps(uploadedSave, storedSave, outputSaveNumber, opt);

        outputSaveData!.SslValue.saveId = outputSaveNumber;
        
        return outputSaveData;
    }

    private SaveData MergeSaveDataAllMaps(Save uploadedSave, Save storedSave, int outputSaveNumber, MergeOptions opt)
    {
        var uploadedSaveData = uploadedSave.SaveData!.SslValue;
        var storedSaveData = storedSave.SaveData!.SslValue;
        
        if (opt.HasFlag(MergeOptions.MissionProgress))
        {
            // cargoLoadingCount merging
            foreach (var (key, value) in storedSaveData.cargoLoadingCounts)
            {
                uploadedSaveData.cargoLoadingCounts[key] = value;
            }

            // discoveredObjectives merging
            uploadedSaveData.discoveredObjectives.AddRange(storedSaveData.discoveredObjectives);

            // objectiveStates merging
            foreach (var (key, value) in storedSaveData.objectiveStates)
            {
                uploadedSaveData.objectiveStates[key] = value;
            }

            // savedCargoNeedToBeRemovedOnRestart merging
            foreach (var (key, value) in storedSaveData.savedCargoNeedToBeRemovedOnRestart)
            {
                uploadedSaveData.savedCargoNeedToBeRemovedOnRestart[key] = value;
            }

            // hiddenCargoes merging
            foreach (var (key, value) in storedSaveData.hiddenCargoes)
            {
                uploadedSaveData.hiddenCargoes[key] = value;
            }

            // persistentProfileData.contestTimes merging
            foreach (var (key, value) in storedSaveData.persistentProfileData.contestTimes)
            {
                uploadedSaveData.persistentProfileData.contestTimes.TryAdd(key, value);
            }

            // persistentProfileData.contestLastTimes merging
            foreach (var (key, value) in storedSaveData.persistentProfileData.contestLastTimes)
            {
                uploadedSaveData.persistentProfileData.contestLastTimes.TryAdd(key, value);
            }

            // finishedObjs merging
            uploadedSaveData.finishedObjs.AddRange(storedSaveData.finishedObjs);
        }
        
        if (opt.HasFlag(MergeOptions.MissionProgress) || opt.HasFlag(MergeOptions.MapProgress))
        {
            // visitedLevels merging
            uploadedSaveData.visitedLevels.AddRange(storedSaveData.visitedLevels);
            
            // persistentProfileData.knownRegions merging
            uploadedSaveData.persistentProfileData.knownRegions.AddRange(storedSaveData.persistentProfileData.knownRegions);
        }

        if (opt.HasFlag(MergeOptions.MapProgress))
        {
            // watchPointsData merging
            foreach (var (key, value) in storedSaveData.watchPointsData.data)
            {
                uploadedSaveData.watchPointsData.data[key] = value;
            }

            // levelGarageStatuses merging
            foreach (var (key, value) in storedSaveData.levelGarageStatuses)
            {
                uploadedSaveData.levelGarageStatuses[key] = value;
            }
            
            // discoveredObjects merging
            uploadedSaveData.discoveredObjects.AddRange(storedSaveData.discoveredObjects);
            
            // upgradableGarages merging
            foreach (var (key, value) in storedSaveData.upgradableGarages)
            {
                uploadedSaveData.upgradableGarages[key] = value;
            }
        }
        
        if (opt.HasFlag(MergeOptions.DiscoveredVehiclesUpgrades))
        {
            MergeDiscoveredTrucksAndObjects(uploadedSaveData, storedSaveData);
        }
        
        if (opt.HasFlag(MergeOptions.GarageContents))
        {
            foreach (var (key, value) in storedSaveData.garagesData)
            {
                uploadedSaveData.garagesData[key] = value;
            }
        }
        
        return new SaveData
        {
            SslType = uploadedSave.SaveData.SslType,
            SslValue = uploadedSaveData
        };
    }

    private SaveData MergeSaveDataWithSpecifiedMaps(Save uploadedSave, Save storedSave, int outputSaveNumber,
        MergeOptions opt, string[] mergedMaps)
    {
        var uploadedSaveData = uploadedSave.SaveData!.SslValue;
        var storedSaveData = storedSave.SaveData!.SslValue;

        foreach (var mergedMap in mergedMaps)
        {
            var levelLowerPrefix = $"level_{mergedMap.ToLowerInvariant()}_";
            var levelUpperPrefix = $"{mergedMap.ToUpperInvariant()}_";
            
            if (opt.HasFlag(MergeOptions.MissionProgress))
            {
                // cargoLoadingCount merging
                foreach (var key in storedSaveData.cargoLoadingCounts.Keys)
                {
                    if (key.StartsWith(levelLowerPrefix, StringComparison.Ordinal))
                    {
                        uploadedSaveData.cargoLoadingCounts[key] = storedSaveData.cargoLoadingCounts[key];
                    }
                }

                // discoveredObjectives merging
                uploadedSaveData.discoveredObjectives.AddRange(
                    storedSaveData.discoveredObjectives
                        .Where(objId => objId.StartsWith(levelUpperPrefix, StringComparison.Ordinal))
                );

                // objectiveStates merging
                foreach (var key in storedSaveData.objectiveStates.Keys)
                {
                    if (key.StartsWith(levelUpperPrefix, StringComparison.Ordinal))
                        uploadedSaveData.objectiveStates[key] = storedSaveData.objectiveStates[key];
                }

                // savedCargoNeedToBeRemovedOnRestart merging
                foreach (var key in storedSaveData.savedCargoNeedToBeRemovedOnRestart.Keys)
                {
                    if (key.StartsWith(levelUpperPrefix, StringComparison.Ordinal))
                        uploadedSaveData.savedCargoNeedToBeRemovedOnRestart[key] = storedSaveData.savedCargoNeedToBeRemovedOnRestart[key];
                }

                // hiddenCargoes merging
                foreach (var key in storedSaveData.hiddenCargoes.Keys)
                {
                    if (key.StartsWith(levelUpperPrefix, StringComparison.Ordinal))
                        uploadedSaveData.hiddenCargoes[key] = storedSaveData.hiddenCargoes[key];
                }

                // persistentProfileData.contestTimes merging
                foreach (var key in storedSaveData.persistentProfileData.contestLastTimes.Keys)
                {
                    if (key.StartsWith(levelUpperPrefix, StringComparison.Ordinal) && 
                        !uploadedSaveData.persistentProfileData.contestTimes.ContainsKey(key))
                    {
                        uploadedSaveData.persistentProfileData.contestTimes[key] = storedSaveData.persistentProfileData.contestTimes[key];
                    }
                }

                // persistentProfileData.contestLastTimes merging
                foreach (var key in storedSaveData.persistentProfileData.contestLastTimes.Keys)
                {
                    if (key.StartsWith(levelUpperPrefix, StringComparison.Ordinal) && 
                        !uploadedSaveData.persistentProfileData.contestLastTimes.ContainsKey(key))
                    {
                        uploadedSaveData.persistentProfileData.contestLastTimes[key] = storedSaveData.persistentProfileData.contestLastTimes[key];
                    }
                }

                // finishedObjs merging
                uploadedSaveData.finishedObjs.AddRange(
                    storedSaveData.finishedObjs
                        .Where(objId => objId.StartsWith(levelUpperPrefix, StringComparison.Ordinal))
                );
            }

            if (opt.HasFlag(MergeOptions.MissionProgress) || opt.HasFlag(MergeOptions.MapProgress))
            {
                // visitedLevels merging
                uploadedSaveData.visitedLevels.AddRange(
                    storedSaveData.visitedLevels
                        .Where(level => level.StartsWith(levelLowerPrefix, StringComparison.Ordinal))
                );
                
                // persistentProfileData.knownRegions merging
                uploadedSaveData.persistentProfileData.knownRegions.AddRange(
                    storedSaveData.persistentProfileData.knownRegions
                        .Where(region => region.StartsWith(mergedMap, StringComparison.CurrentCultureIgnoreCase))
                );
            }

            if (opt.HasFlag(MergeOptions.MapProgress))
            {
                // watchPointsData merging
                foreach (var key in storedSaveData.watchPointsData.data.Keys)
                {
                    if (key.StartsWith(levelLowerPrefix, StringComparison.Ordinal))
                        uploadedSaveData.watchPointsData.data[key] = storedSaveData.watchPointsData.data[key];
                }

                // levelGarageStatuses merging
                foreach (var key in storedSaveData.levelGarageStatuses.Keys)
                {
                    if (key.StartsWith(levelLowerPrefix, StringComparison.Ordinal))
                        uploadedSaveData.levelGarageStatuses[key] = storedSaveData.levelGarageStatuses[key];
                }
                
                // discoveredObjects merging
                uploadedSaveData.discoveredObjects.AddRange(
                    storedSaveData.discoveredObjects
                        .Where(objId => objId.StartsWith(levelUpperPrefix, StringComparison.Ordinal))
                );
                
                // upgradableGarages merging
                foreach (var key in storedSaveData.upgradableGarages.Keys)
                {
                    if (key.StartsWith(levelLowerPrefix, StringComparison.Ordinal))
                        uploadedSaveData.upgradableGarages[key] = storedSaveData.upgradableGarages[key];
                }
            }

            if (opt.HasFlag(MergeOptions.DiscoveredVehiclesUpgrades))
            {
                MergeDiscoveredTrucksAndObjects(uploadedSaveData, storedSaveData);
            }

            if (opt.HasFlag(MergeOptions.GarageContents))
            {
                foreach (var key in storedSaveData.garagesData.Keys)
                {
                    if (key.StartsWith(levelLowerPrefix, StringComparison.Ordinal))
                        uploadedSaveData.garagesData[key] = storedSaveData.garagesData[key];
                }
            }
        }

        return new SaveData
        {
            SslType = uploadedSave.SaveData.SslType,
            SslValue = uploadedSaveData
        };
    }
    
    private void MergeDiscoveredTrucksAndObjects(SslValue uploadedSaveData, SslValue storedSaveData)
    {
        // Unlocked vehicles mergin
        var storedUnlockedItems = storedSaveData.persistentProfileData.unlockedItemNames.Keys.ToHashSet();
        var uploadedUnlockedItems = uploadedSaveData.persistentProfileData.unlockedItemNames.Keys.ToHashSet();
                
        var missingUnlockedItems = storedUnlockedItems.Except(uploadedUnlockedItems);
        foreach (var item in missingUnlockedItems)
        {
            uploadedSaveData.persistentProfileData.unlockedItemNames[item] = true;
        }
                
        // Discovered upgrades merging
        var storedSaveKeys = storedSaveData.upgradesGiverData.Keys.ToHashSet();
        var uploadedSaveKeys = uploadedSaveData.upgradesGiverData.Keys.ToHashSet();
                
        var commonKeys = storedSaveKeys.Intersect(uploadedSaveKeys);
        var storedOnlyKeys = storedSaveKeys.Except(uploadedSaveKeys);
                
        foreach (var key in commonKeys)
        {
            var storedUpgrades = storedSaveData.upgradesGiverData[key];
            var uploadedUpgrades = uploadedSaveData.upgradesGiverData[key];
                    
            foreach (var upgradeKey in storedUpgrades.Keys)
            {
                if (!uploadedUpgrades.ContainsKey(upgradeKey) || storedUpgrades[upgradeKey] > uploadedUpgrades[upgradeKey])
                {
                    uploadedUpgrades[upgradeKey] = storedUpgrades[upgradeKey];
                }
            }
        }
                
        foreach (var key in storedOnlyKeys)
        {
            uploadedSaveData.upgradesGiverData[key] = storedSaveData.upgradesGiverData[key];
        }
        
        uploadedSaveData.persistentProfileData.newTrucks = uploadedSaveData.persistentProfileData.newTrucks.Union(storedSaveData.persistentProfileData.newTrucks).ToHashSet();

        uploadedSaveData.persistentProfileData.discoveredUpgrades = Helpers.MergeDictionaries(
            uploadedSaveData.persistentProfileData.discoveredUpgrades,
            storedSaveData.persistentProfileData.discoveredUpgrades
        );
    }

    [GeneratedRegex(@"level_(\w+?)_(\d+?)_\d+")]
    private static partial Regex KnownRegionRegex();
}
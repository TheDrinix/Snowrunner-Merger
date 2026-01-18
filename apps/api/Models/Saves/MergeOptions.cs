namespace SnowrunnerMerger.Api.Models.Saves;

[Flags]
public enum MergeOptions
{
    MissionProgress = 1 << 0,
    ContestTimes = 1 << 1,
    MapProgress = 1 << 2,
    DiscoveredVehicles = 1 << 3,
    UnlockedUpgrades = 1 << 4,
    GarageContents = 1 << 5,
    VehiclesInWorld = 1 << 6,
    All = MissionProgress | ContestTimes | MapProgress | DiscoveredVehicles | UnlockedUpgrades | GarageContents | VehiclesInWorld
}
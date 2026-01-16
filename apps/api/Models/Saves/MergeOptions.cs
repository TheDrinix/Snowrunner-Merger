namespace SnowrunnerMerger.Api.Models.Saves;

[Flags]
public enum MergeOptions
{
    MissionProgress = 1 << 0,
    MapProgress = 1 << 1,
    DiscoveredVehicles = 1 << 2,
    UnlockedUpgrades = 1 << 3,
    GarageContents = 1 << 4,
    VehiclesInWorld = 1 << 5,
    All = MissionProgress | MapProgress | DiscoveredVehicles | UnlockedUpgrades | GarageContents | VehiclesInWorld
}
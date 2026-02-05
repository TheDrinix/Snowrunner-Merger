namespace SnowrunnerMerger.Api.Models.Saves;

[Flags]
public enum MergeOptions
{
    MissionProgress = 1 << 0,
    ContestTimes = 1 << 1,
    MapProgress = 1 << 2,
    DiscoveredVehiclesUpgrades = 1 << 3,
    GarageContents = 1 << 5,
    VehiclesInWorld = 1 << 6,
    All = MissionProgress | ContestTimes | MapProgress | DiscoveredVehiclesUpgrades | GarageContents | VehiclesInWorld,
    AllExceptGarageContents = MissionProgress | ContestTimes | MapProgress | DiscoveredVehiclesUpgrades | VehiclesInWorld
}
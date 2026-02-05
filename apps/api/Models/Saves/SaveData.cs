namespace SnowrunnerMerger.Api.Models.Saves;

public class SaveData
{
    public string SslType { get; set; }
    public SslValue SslValue { get; set; }
}

public class SslValue
{
    public string lastLoadedLevel { get; set; }
    public bool objectivesValidated { get; set; }
    public int objVersion { get; set; }
    public int saveId { get; set; }
    public Dictionary<string, object> objectiveStates { get; set; }
    public int birthVersion { get; set; }
    public Dictionary<string, Dictionary<string, int>> upgradesGiverData { get; set; }
    public string worldConfiguration { get; set; }
    public int gameDifficultyMode { get; set; }
    public object modTruckOnLevels { get; set; }
    public Dictionary<string, int> levelGarageStatuses { get; set; }
    public bool isHardMode { get; set; }
    public object forcedModelStates { get; set; }
    public List<string> viewedUnactivatedObjectives { get; set; }
    public int lastLevelState { get; set; }
    public double gameTime { get; set; }
    public Dictionary<string, object> hiddenCargoes { get; set; }
    public int metricSystem { get; set; }
    public object garagesShopData { get; set; }
    public object gameDifficultySettings { get; set; }
    public Dictionary<string, string[]> savedCargoNeedToBeRemovedOnRestart { get; set; }
    public object gameStat { get; set; }
    public HashSet<string> finishedObjs { get; set; }
    public List<object> givenTrialRewards { get; set; }
    public HashSet<string> discoveredObjectives { get; set; }
    public string trackedObjective { get; set; }
    public object waypoints { get; set; }
    public Dictionary<string, object> garagesData { get; set; }
    public object modTruckRefundValues { get; set; }
    public WatchPointsData watchPointsData { get; set; }
    public HashSet<string> visitedLevels { get; set; }
    public Dictionary<string, object> cargoLoadingCounts { get; set; }
    public Dictionary<string, object> upgradableGarages { get; set; }
    public PersistentProfileData persistentProfileData { get; set; }
    public object modTruckTypesRefundValues { get; set; }
    public SaveTime saveTime { get; set; }
    public object tutorialStates { get; set; }
    public object justDiscoveredObjects { get; set; }
    public HashSet<string> discoveredObjects { get; set; }
    public object gameStatByRegion { get; set; }
    public bool isFirstGarageDiscovered { get; set; }
    public int lastPhantomMode { get; set; }
}

public class PersistentProfileData
{
    public int money { get; set; }
    public object distance { get; set; }
    public List<string> dlcNotes { get; set; }
    public object ownedTrucks { get; set; }
    public int experience { get; set; }
    public HashSet<string> newTrucks { get; set; }
    public object contestAttempts { get; set; }
    public int customizationRefundMoney { get; set; }
    public List<object> trucksInWarehouse { get; set; }
    public int rank { get; set; }
    public object refundTruckDescs { get; set; }
    public Dictionary<string, int> contestLastTimes { get; set; }
    public bool isNewProfile { get; set; }
    public Dictionary<string, object> discoveredTrucks { get; set; }
    public Dictionary<string, object> discoveredUpgrades { get; set; }
    public object damagableAddons { get; set; }
    public Dictionary<string, int> contestTimes { get; set; }
    public HashSet<string> knownRegions { get; set; }
    public Dictionary<string, bool> unlockedItemNames { get; set; }
    public List<object> refundGarageTruckDescs { get; set; }
    public object addons { get; set; }
    public int refundMoney { get; set; }
    public object userId { get; set; }
}

public class SaveTime
{
    public string timestamp { get; set; }
}

public class WatchPointsData
{
    public Dictionary<string, object> data { get; set; }
}
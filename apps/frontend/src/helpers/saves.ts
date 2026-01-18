const saveFileRegex = /CompleteSave\d?\.dat/m;
const mapFileRegex = /\b(\d_)?(fog|sts)_.*\.dat\b/m;

export const validateSaveFiles = (fileNames: string[]): number[] => {
    let saveFileNumbers: number[] = [];
    let mapFilesCount = 0;

    fileNames.forEach((filename) => {
        if (saveFileRegex.test(filename)) {
            const num = filename.match(/\d/g);

            const saveNumber = parseInt(num?.length ? num[0] : '0');

            saveFileNumbers.push(saveNumber);
        } else if (mapFileRegex.test(filename)) {
            mapFilesCount++;
        }
    })

    return mapFilesCount > 0 ? saveFileNumbers : [];
}

export const generateSaveRegex = (saveNumber: number): RegExp => {
    const prefix = saveNumber ? saveNumber + '_' : '';

    const regexString = `(CompleteSave${saveNumber || ''}\\.dat)|(\\b${prefix}(fog|sts)_.*\\.dat\\b)`;

    return new RegExp(regexString, 'm');
}

export enum SaveMergingOptions {
    MissionProgress = 1 << 0,
    ContestTimes = 1 << 1,
    MapProgress = 1 << 2,
    DiscoveredVehicles = 1 << 3,
    UnlockedUpgrades = 1 << 4,
    GarageContents = 1 << 5,
    VehiclesInWorld = 1 << 6,
    All = MissionProgress | ContestTimes | MapProgress | DiscoveredVehicles | UnlockedUpgrades | GarageContents | VehiclesInWorld
}
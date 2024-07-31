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

    const regexString = `(CompleteSave${saveNumber}\\.dat)|(\\b${prefix}(fog|sts)_.*\\.dat\\b)`;

    return new RegExp(regexString, 'm');
}
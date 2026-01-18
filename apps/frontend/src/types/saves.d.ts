import {SaveMergingOptions} from "@/helpers/saves";

export interface StoredSave {
    id: string;
    description: string;
    uploadedAt: Date;
    discoveredMaps: DiscoveredMap[];
}

export interface DiscoveredMap {
    id: string;
    name: string;
}

export interface SaveMergingConfig {
    options: SaveMergingOptions;
    maps: string[];
}
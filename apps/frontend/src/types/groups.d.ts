import type {StoredSave} from "@/types/saves";

export interface GroupData {
    id: stirng;
    name: string;
    members: GroupMember[];
    owner: GroupMember;
}

export interface Group {
    id: stirng;
    name: string;
    members: GroupMember[];
    owner: GroupMember;
    saves: StoredSave[];
    lastLoadedSavesAt?: Date;
}

export interface GroupMember {
    id: string;
    name: string;
}

export interface GroupsStore {
    groups: Map<string, Group>
}
export interface Group {
    id: stirng;
    name: string;
    members: GroupMember[];
    owner: GroupMember;
}

export interface GroupMember {
    id: string;
    name: string;
}

export interface GroupsStore {
    groups: Map<string, Group>
}
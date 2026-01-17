import {defineStore} from "pinia";
import type {Group, GroupData, GroupsStore} from "@/types/groups";
import {useUserStore} from "@/stores/userStore";
import type {StoredSave} from "@/types/saves";
import { ref } from "vue";

export const useGroupsStore = defineStore("groups", () => {
    const groups = ref<Map<string, Group>>(new Map());
    const userStore = useUserStore();

    // Getters (as functions)
    const getGroup = (id: string) => {
        return groups.value.get(id);
    };

    const getOwnedGroups = () => {
        const ownedGroups= Array.from(groups.value.values()).filter(
            (group) => group.owner.id === userStore.user?.id
        );
        
        console.log(ownedGroups);
        console.log(groups.value)
        
        return ownedGroups;
    };

    const getJoinedGroups = () => {
        return Array.from(groups.value.values()).filter(
            (group) => group.owner.id !== userStore.user?.id
        );
    };

    const isGroupOwner = (id: string) => {
        const group = groups.value.get(id);
        return group?.owner.id === userStore.user?.id;
    };

    // Actions
    function storeGroups(groupsData: GroupData[]) {
        groupsData.forEach((data) => {
            const group: Group = {
                ...data,
                saves: [],
            };
            groups.value.set(group.id, group);
        });
    }

    function storeGroup(groupData: GroupData) {
        const group: Group = { ...groupData, saves: [] };
        groups.value.set(group.id, group);
    }

    function storeGroupSaves(groupId: string, saves: StoredSave[]) {
        const group = groups.value.get(groupId);
        if (group) {
            group.saves = saves;
            group.lastLoadedSavesAt = new Date();
            groups.value.set(groupId, group);
        }
    }

    function deleteGroupSave(groupId: string, saveIdx: number) {
        const group = groups.value.get(groupId);
        if (group) {
            group.saves.splice(saveIdx, 1);
            groups.value.set(groupId, group);
        }
    }

    function removeGroup(groupId: string) {
        groups.value.delete(groupId);
    }

    function clearSaves(groupId: string) {
        const group = groups.value.get(groupId);
        if (group) {
            group.saves = [];
            group.lastLoadedSavesAt = new Date(0);
            groups.value.set(groupId, group);
        }
    }

    function clearStore() {
        groups.value = new Map();
    }

    return {
        groups,
        getGroup,
        getOwnedGroups,
        getJoinedGroups,
        isGroupOwner,
        storeGroups,
        storeGroup,
        storeGroupSaves,
        deleteGroupSave,
        removeGroup,
        clearSaves,
        clearStore,
    };
});

/*
export const useGroupsStore = defineStore('groups', {
    state: (): GroupsStore => ({...defaultState}),
    getters: {
        getGroup: (state) => (id: string) => {
            return state.groups.get(id);
        },
        getOwnedGroups: (state) => {
            const userStore = useUserStore();

            return Array.from(state.groups.values())
                .filter(group => group.owner.id === userStore.user?.id);
        },
        getJoinedGroups: (state) => {
            const userStore = useUserStore();

            return Array.from(state.groups.values())
                .filter(group => group.owner.id !== userStore.user?.id);
        },
        isGroupOwner: (state) => (id: string) => {
            const userStore = useUserStore();

            const group = state.groups.get(id);

            return group?.owner.id === userStore.user?.id;
        }
    },
    actions: {
        storeGroups(groups: GroupData[]) {
            groups.forEach(data => {
                const group = {
                    ...data,
                    saves: []
                };

                this.groups.set(group.id, group);
            });
        },
        storeGroup(group: GroupData) {
            this.groups.set(group.id, {...group, saves: []});
        },
        storeGroupSaves(groupId: string, saves: StoredSave[]) {
            const group = this.groups.get(groupId);

            if (group) {
                group.saves = saves;
                group.lastLoadedSavesAt = new Date();

                this.groups.set(groupId, group);
            }
        },
        deleteGroupSave(groupId: string, saveIdx: number) {
            const group = this.groups.get(groupId);

            if (group) {
                group.saves.splice(saveIdx, 1);

                this.groups.set(groupId, group);
            }
        },
        removeGroup(groupId: string) {
          this.groups.delete(groupId);
        },
        clearSaves(groupId: string) {
            const group = this.groups.get(groupId);

            if (group) {
                group.saves = [];
                group.lastLoadedSavesAt = new Date(0);

                this.groups.set(groupId, group);
            }
        },
        clearStore() {
            this.$state = {...defaultState};
        }
    }
});*/
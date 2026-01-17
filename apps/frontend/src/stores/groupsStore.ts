import {defineStore} from "pinia";
import type {Group, GroupData} from "@/types/groups";
import {useUserStore} from "@/stores/userStore";
import type {StoredSave} from "@/types/saves";
import {ref} from "vue";
import { useHttp } from "@/composables/useHttp";

export const useGroupsStore = defineStore("groups", () => {
    const groups = ref<Map<string, Group>>(new Map());
    const isLoading = ref<boolean>(false);
    const userStore = useUserStore();
    const http = useHttp();

    // Getters (as functions)
    const getGroup = (id: string) => {
        return groups.value.get(id);
    };

    const getOwnedGroups = () => {
        return Array.from(groups.value.values()).filter(
            (group) => group.owner.id === userStore.user?.id
        );
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
    
    async function fetchGroupSaves(groupId: string) {
        const group = groups.value.get(groupId);
        
        if (!group || (group.lastLoadedSavesAt && group.lastLoadedSavesAt.getTime() < Date.now() - 1000 * 60 * 5)) return;

        isLoading.value = true;
        try {
            const res = await http.get<StoredSave[]>(`/groups/${groupId}/saves`);
            if (res) {
                storeGroupSaves(groupId, res.data);
            }

            isLoading.value = false;
        } catch (e) {
            isLoading.value = false;
            throw e;
        }
    }

    return {
        groups,
        isLoading,
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
        fetchGroupSaves
    };
});
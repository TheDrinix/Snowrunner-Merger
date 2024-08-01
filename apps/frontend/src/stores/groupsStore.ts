import {defineStore} from "pinia";
import type {GroupData, GroupsStore} from "@/types/groups";
import {useUserStore} from "@/stores/userStore";
import type {StoredSave} from "@/types/saves";

const defaultState: GroupsStore = {
    groups: new Map()
}

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
        }
    }
});
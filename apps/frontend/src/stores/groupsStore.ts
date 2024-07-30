import {defineStore} from "pinia";
import type {Group, GroupsStore} from "@/types/groups";
import {useUserStore} from "@/stores/userStore";

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
        storeGroups(groups: Group[]) {
            groups.forEach(group => {
                this.groups.set(group.id, group);
            });
        }
    }
});
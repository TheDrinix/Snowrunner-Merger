<script setup lang="ts">
import {useRoute, useRouter, type RouteLocationRaw} from "vue-router";
import {computed, ref} from "vue";
import {useGroupsStore} from "@/stores/groupsStore";
import GroupSave from "@/components/groups/GroupSave.vue";
import GroupSaveLoading from "@/components/groups/GroupSaveLoading.vue";
import {useUserStore} from "@/stores/userStore";
import GroupLeaveModal from "@/components/groups/GroupLeaveModal.vue";
import GroupDeleteModal from "@/components/groups/GroupDeleteModal.vue";
import GroupHeader from "@/components/groups/header/GroupHeader.vue";
import type { NavigationLink } from "@/types/navigation";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();
const userStore = useUserStore();


const groupId = computed(() => {
  return route.params.id as string;
});

const group = computed(() => {
  return groupsStore.getGroup(groupId.value);
});

const saves = computed(() => {
  return group.value?.saves || [];
});

if (!group.value) {
  router.push({name: 'groups'});
}

const links = computed<NavigationLink[]>(() => {
  return [
    { name: 'Groups', to: { name: 'groups' } },
    { name: group.value?.name || '', to: { name: 'group', params: { id: groupId.value } } }
  ];
});

const isOwner = computed(() => {
  return group.value?.ownerId === userStore.user?.id;
});


const loading = groupsStore.isLoading;

groupsStore.fetchGroupSaves(groupId.value);
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 py-8">
    <div class="flex flex-col md:flex-row md:items-end justify-between gap-4 mb-8">
      <GroupHeader :groupName="group?.name || ''" :loading="loading" :links />

      <div v-if="!loading" class="flex gap-2">
        <template v-if="isOwner">
          <RouterLink
              v-if="saves.length < 3"
              :to="{ name: 'group-save-upload' }"
              class="btn btn-secondary btn-sm shadow-lg"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0l-4 4m4-4v12" /></svg>
            Upload New Save
          </RouterLink>
          <GroupDeleteModal :group="group!" />
        </template>
        <GroupLeaveModal v-else :group="group!" />
      </div>
    </div>

    <div class="grid grid-cols-1 gap-6">
      <div v-if="!loading" class="space-y-6">
        <GroupSave
            v-for="(save, idx) in saves"
            :key="save.id"
            :save="save"
            :idx="idx"
            :groupId="groupId"
            :isOwner="isOwner"
            :totalSaves="saves.length"
        />

        <div
            v-for="i in (3 - saves.length)"
            :key="'empty-' + i"
            class="border-2 border-dashed border-base-300 rounded-2xl p-8 flex flex-col items-center justify-center bg-base-200/50 opacity-50"
        >
          <span class="text-sm font-bold uppercase tracking-widest opacity-40">Empty Slot {{ saves.length + i }}</span>
          <p v-if="isOwner" class="text-xs mt-1">Owner can upload a save to this slot</p>
        </div>
      </div>

      <div v-else class="space-y-6">
        <GroupSaveLoading v-for="i in 3" :key="i" />
      </div>
    </div>
  </div>
</template>

<style scoped>
@media (min-width: 1024px) {
  .join.join-vertical > :where(*:not(:first-child)):is(.btn) {
    margin-top: 0;
  }
}
</style>
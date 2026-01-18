<script setup lang="ts">
import type {StoredSave} from "@/types/saves";
import {useTimeAgo} from "@vueuse/core";
import {useToaster} from "@/stores/toastStore";
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";

const props = defineProps<{
  save: StoredSave,
  idx: number,
  groupId: string,
  isOwner: boolean,
  totalSaves: number
}>();

const { createToast } = useToaster();
const http = useHttp();
const groupsStore = useGroupsStore();

const timeAgo = useTimeAgo(props.save.uploadedAt);

const handleSaveDelete = async (saveIdx: number) => {
  const saveId = props.save.id;

  try {
    await http.delete(`/groups/${props.groupId}/saves/${saveId}`);

    groupsStore.deleteGroupSave(props.groupId, saveIdx);
  } catch (e: any) {
    if (e.response.data.title) {
      createToast(e.response.data.title, 'error');
    }
  }
}
</script>

<template>
  <div class="card bg-base-100 shadow-xl border border-base-200 overflow-hidden hover:border-secondary/30 transition-all">
    <div class="flex flex-col lg:flex-row">

      <div class="p-6 flex-grow border-b lg:border-b-0 lg:border-r border-base-200">
        <div class="flex items-center gap-3 mb-2">
          <div class="badge badge-secondary badge-outline font-bold">SLOT {{ idx + 1 }}</div>
          <span class="text-xs opacity-50 font-mono">{{ timeAgo }}</span>
        </div>

        <p class="text-base-content/80 italic leading-relaxed">
          "{{ save.description || 'No description provided.' }}"
        </p>
      </div>

      <div class="p-6 bg-base-200/30 lg:w-72 flex flex-col justify-center gap-3">
        <RouterLink
            :to="{name: 'group-save-merge', query: { saveNumber: idx }, params: { id: groupId }}"
            class="btn btn-secondary btn-block shadow-md"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" /></svg>
          Merge Save
        </RouterLink>

        <div v-if="isOwner" class="grid grid-cols-2 gap-2">
          <RouterLink
              :to="{name: 'group-save-upload', query: { saveNumber: idx }, params: { id: groupId }}"
              class="btn btn-outline btn-xs"
          >
            Replace
          </RouterLink>
          <button
              @click="() => handleSaveDelete(idx)"
              class="btn btn-outline btn-error btn-xs"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
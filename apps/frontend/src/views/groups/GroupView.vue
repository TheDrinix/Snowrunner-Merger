<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";
import GroupSave from "@/components/groups/GroupSave.vue";
import GroupSaveLoading from "@/components/groups/GroupSaveLoading.vue";
import {useUserStore} from "@/stores/userStore";
import {useToaster} from "@/stores/toastStore";
import GroupLeaveModal from "@/components/groups/GroupLeaveModal.vue";
import GroupDeleteModal from "@/components/groups/GroupDeleteModal.vue";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();
const userStore = useUserStore();
const { createToast } = useToaster();
const http = useHttp();

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

const isOwner = computed(() => {
  return group.value?.ownerId === userStore.user?.id;
});


const loading = groupsStore.isLoading;

groupsStore.fetchGroupSaves(groupId.value);

const handleSaveDelete = async (saveIdx: number) => {
  const saveId = group.value?.saves[saveIdx].id;

  try {
    await http.delete(`/groups/${groupId.value}/saves/${saveId}`);

    groupsStore.deleteGroupSave(groupId.value, saveIdx);
  } catch (e: any) {
    if (e.response.data.title) {
      createToast(e.response.data.title, 'error');
    }
  }
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:2/3 xl:w-1/2 bg-base-200 shadow-xl">
    <div class="card-header flex" v-if="!loading">
      <h2 class="card-title">Save group {{group?.name}}</h2>
      <div class="ml-auto">
        <div class="flex flex-col md:flex-row gap-2 md:gap-4" v-if="isOwner">
          <RouterLink :to="{ name: 'group-save-upload' }" class="btn btn-outline btn-secondary btn-sm w-full md:w-fit">Upload save</RouterLink>
          <GroupDeleteModal :group="group!" />
        </div>
        <div v-else>
          <GroupLeaveModal :group="group!" />
        </div>
      </div>
    </div>
    <div class="card-header" v-else>
      <div class="skeleton h-8 w-36" />
    </div>
    <div class="card-body" v-if="!loading">
      <GroupSave v-for="(save, idx) in saves" :key="save.id" :save="save" :idx="idx" :groupId="groupId">
        <template #actions>
          <div class="join join-vertical lg:join-horizontal">
            <RouterLink :to="{name: 'group-save-merge', query: { saveNumber: idx }, params: { id: groupId }}" class="btn btn-outline btn-secondary join-item">Merge</RouterLink>
            <template v-if="isOwner">
              <RouterLink v-if="group!.saves.length >= 3" :to="{name: 'group-save-upload', query: { saveNumber: idx }, params: { id: groupId }}" class="btn btn-outline btn-secondary join-item">Replace</RouterLink>
              <button class="btn btn-outline btn-secondary join-item" @click="() => handleSaveDelete(idx)">Delete</button>
            </template>
          </div>
        </template>
      </GroupSave>
      <div v-if="!saves.length">
        <p class="text-lg font-medium">There are currently no saves shared in this group.</p>
      </div>
    </div>
    <div class="card-body" v-else>
      <GroupSaveLoading v-for="i in 3" :key="i" />
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
<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useGroupsStore} from "@/stores/groupsStore";
import {useHttp} from "@/composables/useHttp";
import GroupSave from "@/components/groups/GroupSave.vue";
import Modal from "@/components/Modal.vue";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();

const groupId = computed(() => {
  return route.params.id as string;
});

const group = computed(() => {
  return groupsStore.getGroup(groupId.value);
})

if (!group.value) {
  router.push({name: 'groups'});
}

const loading = ref(true);
const http = useHttp();

if (!group.value?.lastLoadedSavesAt || group.value.lastLoadedSavesAt.getTime() < Date.now() - 1000 * 60 * 5) {
  http.get(`/groups/${group.value?.id}/saves`)
    .then(res => {
      groupsStore.storeGroupSaves(group.value?.id, res.data);
      loading.value = false;
    })
    .catch(e => {
      router.push({name: 'groups'});
    });
}

const handleSaveDelete = async (saveIdx: number) => {
  const saveId = group.value?.saves[saveIdx].id;

  try {
    const res = await http.delete(`/groups/${groupId.value}/saves/${saveId}`);

    groupsStore.deleteGroupSave(groupId.value, saveIdx);
  } catch (e) {
    console.error('Failed to delete save');
  }
}

const isConfirmDeleteModalOpen = ref(false);

const handleGroupDelete = async () => {
  try {
    const res = await http.delete(`/groups/${groupId.value}`);

    groupsStore.removeGroup(groupId.value);
  }

  await router.push({name: 'groups'});
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:2/3 xl:w-1/2 bg-base-200 shadow-xl">
    <div class="card-header flex">
      <h2 class="card-title">
        {{ group?.name }}
      </h2>
      <div class="ml-auto flex flex-col md:flex-row items-center gap-2 md:gap-4">
        <RouterLink :to="{ name: 'group-save-upload' }" class="btn btn-outline btn-secondary btn-sm w-full md:w-fit">Upload save</RouterLink>
        <button class="btn btn-outline btn-error btn-sm w-full md:w-fit" @click="isConfirmDeleteModalOpen = true">Delete group</button>
      </div>
      <Modal v-model="isConfirmDeleteModalOpen">
        <h2 class="text-lg">Are you sure you want to delete group {{group?.name}}?</h2>
        <div class="modal-action">
          <button class="btn btn-neutral" @click="isConfirmDeleteModalOpen = false">Cancel</button>
          <button class="btn btn-error" @click="handleGroupDelete">Delete</button>
        </div>
      </Modal>
    </div>
    <div class="card-body">
      <GroupSave v-if="group?.saves.length" v-for="(save, idx) in group.saves" :key="save.id" :save="save" :idx="idx" :groupId="groupId">
        <template #actions>
          <RouterLink v-if="group.saves.length >= 3" :to="{name: 'group-save-upload', query: { saveNumber: idx }, params: { id: groupId }}" class="btn btn-outline btn-secondary">Replace</RouterLink>
          <button class="btn btn-outline btn-secondary w-full" @click="() => handleSaveDelete(idx)">Delete</button>
        </template>
      </GroupSave>
      <h2 v-else class="text-lg font-medium">There are currently no stored saves in this group</h2>
    </div>
  </div>
</template>

<style scoped>

</style>
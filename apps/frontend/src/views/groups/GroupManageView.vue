<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useGroupsStore} from "@/stores/groupsStore";
import {useHttp} from "@/composables/useHttp";
import GroupSave from "@/components/groups/GroupSave.vue";

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
      if (res.status < 300) {
        groupsStore.storeGroupSaves(group.value?.id, res.data);
        loading.value = false;
      } else router.push({name: 'groups'});
    });
}

const handleSaveDelete = async (saveIdx: number) => {
  const saveId = group.value?.saves[saveIdx].id;

  const res = await http.delete(`/groups/${groupId.value}/saves/${saveId}`)

  if (res.status < 300) {
    groupsStore.deleteGroupSave(groupId.value, saveIdx);
  }
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:2/3 xl:w-1/2 bg-base-200 shadow-xl">
    <div class="card-header">
      <h2 class="card-title">
        {{ group?.name }}
      </h2>
    </div>
    <div class="card-body" v-if="group?.saves">
      <GroupSave v-for="(save, idx) in group.saves" :key="save.id" :save="save" :idx="idx" :groupId="groupId">
        <template #actions>
          <RouterLink v-if="group.saves.length >= 3" :to="{name: 'group-save-upload', query: { saveNumber: idx }, params: { id: groupId }}" class="btn btn-outline btn-secondary">Replace</RouterLink>
          <button class="btn btn-outline btn-secondary w-full" @click="() => group ? handleSaveDelete(idx) : ''">Delete</button>
        </template>
      </GroupSave>
    </div>
  </div>
</template>

<style scoped>

</style>
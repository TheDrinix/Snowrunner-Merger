<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";
import GroupSave from "@/components/groups/GroupSave.vue";
import GroupSaveLoading from "@/components/groups/GroupSaveLoading.vue";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();

const groupId = computed(() => {
  return route.params.id as string;
});

const group = computed(() => {
  return groupsStore.getGroup(groupId.value);
});

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
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:2/3 xl:w-1/2 bg-base-200 shadow-xl">
    <div class="card-header">
      <h2 class="card-title" v-if="!loading">Save group {{group?.name}}</h2>
      <div class="skeleton h-8 w-36" v-else></div>
    </div>
    <div class="card-body" v-if="!loading">
      <GroupSave v-for="(save, idx) in group?.saves" :key="save.id" :save="save" :idx="idx" :groupId="groupId">
        <template #actions>
          <RouterLink :to="{name: 'group-save-merge', query: { saveNumber: idx }, params: { id: groupId }}" class="btn btn-outline btn-secondary">Merge</RouterLink>
        </template>
      </GroupSave>
    </div>
    <div class="card-body" v-else>
      <GroupSaveLoading v-for="i in 3" :key="i" />
    </div>
  </div>
</template>

<style scoped>

</style>
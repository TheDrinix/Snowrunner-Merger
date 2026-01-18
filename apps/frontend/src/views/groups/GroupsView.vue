<script setup lang="ts">
import {computed, ref} from "vue";
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";
import type {GroupData as IGroup} from "@/types/groups";
import Group from "@/components/groups/Group.vue";
import GroupLoading from "@/components/groups/GroupLoading.vue";
import {useRouter} from "vue-router";
import {useToaster} from "@/stores/toastStore";

const http = useHttp();
const groupsStore = useGroupsStore();
const router = useRouter();

const isLoading = ref(!groupsStore.groups.size);
const {createToast} = useToaster();

http.get<IGroup[]>("/groups")
  .then(res => {
    groupsStore.storeGroups(res.data);

    isLoading.value = false;
  })
  .catch(e => {
    router.push({name: "login"});
  });

const joinedGroups = computed(() => groupsStore.getJoinedGroups());
const ownedGroups = computed(() => groupsStore.getOwnedGroups());

const groupJoinCode = ref("");

const isValidCode = computed(() => {
  const regex = /^[0-9A-Z]{10,}$/mi

  return regex.test(groupJoinCode.value);
})

const handleJoinGroup = () => {
  const body = {
    inviteCode: groupJoinCode.value,
  }
  
  http.post(`/groups/join`, body)
    .then(res => {
      groupsStore.storeGroup(res.data);
      groupJoinCode.value = "";
    })
    .catch(e => {
      if (e.response.data.title) {
        createToast(e.response.data.title, 'error');
      }
    });
}
</script>

<template>
  <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 p-4 lg:p-0">

    <div class="lg:col-span-7 flex flex-col gap-4">
      <div class="card bg-base-100 border border-base-300 shadow-sm">
        <div class="card-body p-6">
          <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6">
            <h2 class="card-title text-2xl font-bold text-primary">Joined Groups</h2>

            <form @submit.prevent="handleJoinGroup" class="w-full md:w-auto">
              <div class="join w-full shadow-sm">
                <input v-model="groupJoinCode" class="input input-bordered input-sm join-item w-full md:w-40 focus:w-60 transition-all duration-300" placeholder="Join code..." />
                <button :disabled="!isValidCode" class="btn btn-primary btn-sm join-item px-6">Join</button>
              </div>
            </form>
          </div>

          <div v-if="!isLoading" class="space-y-3">
            <template v-if="joinedGroups.length">
              <Group v-for="group in joinedGroups" :key="group.id" :group="group" />
            </template>
            <div v-else class="text-center py-10 opacity-50 border-2 border-dashed border-base-300 rounded-xl">
              <p>You haven't joined any groups yet.</p>
            </div>
          </div>

          <div v-else class="space-y-3">
            <GroupLoading v-for="i in 3" :key="i" />
          </div>
        </div>
      </div>
    </div>

    <div class="lg:col-span-5">
      <div class="card bg-base-200 border border-base-300 shadow-sm sticky top-4">
        <div class="card-body p-6">
          <div class="flex justify-between items-center mb-4">
            <h2 class="card-title text-xl font-bold">Your Groups</h2>
            <div class="badge badge-neutral gap-1 p-3">
              <span class="font-bold text-primary">{{ ownedGroups.length }}</span>
              <span class="opacity-50 text-xs">/ 4 slots</span>
            </div>
          </div>

          <div v-if="!isLoading" class="space-y-3">
            <Group v-for="group in ownedGroups" :key="group.id" :group="group" isOwner />

            <RouterLink 
              :to="{ name: 'group-create' }"
              class="border-2 border-dashed border-base-300 rounded-xl h-20 flex items-center justify-center opacity-40 hover:opacity-100 transition-opacity cursor-pointer group"
              v-for="i in (4 - ownedGroups.length)" :key="'slot' + i"
              tag="div"
            >
              <span class="text-sm font-medium group-hover:text-primary">+ Create New Group</span>
            </RouterLink>
          </div>

          <div v-else class="space-y-3">
            <GroupLoading v-for="i in 2" :key="i" isOwner />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
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
const isLoading = ref(true);
const {createToast} = useToaster();

http.get<IGroup[]>("/groups")
  .then(res => {
    groupsStore.storeGroups(res.data);

    isLoading.value = false;
  })
  .catch(e => {
    router.push({name: "login"});
  });

const joinedGroups = computed(() => groupsStore.getJoinedGroups);
const ownedGroups = computed(() => groupsStore.getOwnedGroups);

const groupJoinCode = ref("");

const isValidCode = computed(() => {
  const regex = /^[0-9A-F]{8}-[0-9A-F]{4}-4[0-9A-F]{3}-[89AB][0-9A-F]{3}-[0-9A-F]{12}$/mi

  return regex.test(groupJoinCode.value);
})

const handleJoinGroup = () => {
  http.post(`/groups/${groupJoinCode.value}/join`)
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
  <div class="flex flex-col lg:flex-row gap-4">
    <div class="h-fit card w-11/12 mx-auto md:w-full bg-base-200 shadow-xl">
      <div class="card-header">
        <h2 class="card-title">Joined groups</h2>
      </div>
      <div class="card-body overflow-y-scroll" v-if="!isLoading">
        <form @submit.prevent="handleJoinGroup">
          <div class="flex flex-col gap-4">
            <div v-if="false" class="alert alert-error"></div>
            <div class="join">
              <div class="w-full">
                <input v-model="groupJoinCode" class="input input-bordered join-item w-full" placeholder="Enter a group code to join" />
              </div>
              <div>
                <button :disabled="!isValidCode" class="btn btn-neutral join-item">Join</button>
              </div>
            </div>
          </div>
        </form>
        <template v-if="joinedGroups.length">
          <hr class="border-none my-0 divider divider-vertical">
          <Group v-for="group in joinedGroups" :key="group.id" :group="group" />
        </template>
      </div>
      <div class="card-body" v-else>
        <div class="skeleton h-8 w-full" />
        <hr class="border-none my-0 divider divider-vertical">
        <GroupLoading v-for="i in 3" :key="i" />
      </div>
    </div>
    <div class="h-fit card w-11/12 mx-auto md:w-full bg-base-200 shadow-xl">
      <div class="card-header">
        <h2 class="card-title">Owned groups</h2>
      </div>
      <div class="card-body" v-if="!isLoading">
        <p v-if="!ownedGroups.length" class="text-lg font-medium">You currently don't own any groups</p>
        <Group v-for="group in ownedGroups" :key="group.id" :group="group" isOwner />
      </div>
      <div class="card-body" v-else>
        <GroupLoading v-for="i in 3" :key="i" isOwner />
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
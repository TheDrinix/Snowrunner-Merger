<script setup lang="ts">
import {computed, ref} from "vue";
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";
import type {Group as IGroup} from "@/types/groups";
import Group from "@/components/groups/Group.vue";
import GroupLoading from "@/components/groups/GroupLoading.vue";
import {useRouter} from "vue-router";

const http = useHttp();
const groupsStore = useGroupsStore();
const router = useRouter();
const isLoading = ref(true);

http.get<IGroup[]>("/groups")
    .then(res => {
        if (res.status < 300) groupsStore.storeGroups(res.data);
        else router.push({name: "login"});

        isLoading.value = false;
    });

const joinedGroups = computed(() => groupsStore.getJoinedGroups);
const ownedGroups = computed(() => groupsStore.getOwnedGroups);

const handleJoinGroup = () => {
  alert('This will eventually do something lol');
}
</script>

<template>
  <div class="flex flex-col lg:flex-row gap-4">
    <div class="h-fit card w-11/12 mx-auto md:w-full bg-base-200 shadow-xl">
      <div class="card-header">
        <h2 class="card-title">Joined groups</h2>
      </div>
      <div class="card-body overflow-y-scroll" v-if="!isLoading">
        <template v-if="joinedGroups.length > 0">
          <form @submit.prevent="handleJoinGroup">
            <div class="flex flex-col gap-4">
              <div v-if="false" class="alert alert-error"></div>
              <div class="join">
                <div class="w-full">
                  <input class="input input-bordered join-item w-full" placeholder="Enter a group code to join" />
                </div>
                <div>
                  <button class="btn btn-neutral join-item">Join</button>
                </div>
              </div>
            </div>
          </form>
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
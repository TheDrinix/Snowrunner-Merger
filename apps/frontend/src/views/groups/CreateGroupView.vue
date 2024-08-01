<script setup lang="ts">
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";
import {useRouter} from "vue-router";
import {computed, ref} from "vue";
import type {GroupData as IGroup} from "@/types/groups";

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

const ownedGroups = computed(() => groupsStore.getOwnedGroups);

const groupName = ref("");

const handleCreateGroup = () => {
  http.post("/groups", {name: groupName.value})
    .then(res => {
      if (res.status < 300) {
        groupsStore.storeGroup(res.data);
        groupName.value = "";

        router.push({name: 'group-manage', params: {id: res.data.id}});
      }
    });
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:2/3 xl:w-1/2 bg-base-200 shadow-xl">
    <div class="card-header">
      <h2 class="card-title">Create a group</h2>
    </div>
    <div class="card-body">
      <div class="alert" role="alert" v-if="ownedGroups.length >= 4">
        <div class="flex-1">
          <span>You have reached the maximum number of groups you can own.</span>
        </div>
      </div>
      <form @submit.prevent="handleCreateGroup">
        <div class="form-control">
          <label class="label">
            <span class="label-text">Group name</span>
          </label>
          <input v-model="groupName" type="text" class="input" placeholder="Enter a group name" />
        </div>
        <div class="flex justify-center mt-4">
          <button :disabled="ownedGroups.length >= 4 || groupName.length < 3" type="submit" class="btn btn-primary btn-wide">Create</button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>

</style>
<script setup lang="ts">
import {useHttp} from "@/composables/useHttp";
import {useGroupsStore} from "@/stores/groupsStore";
import {useRouter} from "vue-router";
import {computed, ref} from "vue";
import type {GroupData as IGroup} from "@/types/groups";
import {useToaster} from "@/stores/toastStore";

const http = useHttp();
const groupsStore = useGroupsStore();
const router = useRouter();
const {createToast} = useToaster();

const isLoading = ref(true);
const err = ref('');
const sentLoading = ref(false);

http.get<IGroup[]>("/groups")
  .then(res => {
    groupsStore.storeGroups(res.data);

    isLoading.value = false;
  })
  .catch(e => {
    router.push({name: "login"});
  });

const ownedGroups = computed(() => groupsStore.getOwnedGroups);

const groupName = ref("");

const handleCreateGroup = () => {
  sentLoading.value = true;

  http.post("/groups", {name: groupName.value})
    .then(res => {
      groupsStore.storeGroup(res.data);
      groupName.value = "";

      router.push({name: 'group-manage', params: {id: res.data.id}});
    })
    .catch((e: any) => {
      if (e.response.status === 401) {
        router.push({name: 'login'});
      } else if (e.response.status === 409) {
        err.value = "You already own maximum amount of groups"
      } else {
        if (e.response.data.title) {
          createToast(e.response.data.title, 'error');
        }
      }
    })
    .finally(() => {
      sentLoading.value = false;
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
        <div v-if="err">
          <div class="alert alert-error">
            {{err}}
          </div>
        </div>
        <div class="form-control">
          <label class="label">
            <span class="label-text">Group name</span>
          </label>
          <input v-model="groupName" type="text" class="input" placeholder="Enter a group name" />
        </div>
        <div class="flex justify-center mt-4">
          <button :disabled="ownedGroups.length >= 4 || groupName.length < 3 || sentLoading" type="submit" class="btn btn-primary btn-wide">
            Create <span v-if="sentLoading" class="loading loading-dots" />
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>

</style>
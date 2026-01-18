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

const ownedGroups = computed(() => groupsStore.getOwnedGroups());

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
  <div class="max-w-xl mx-auto py-12 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300">
      <div class="card-body p-8">
        <h2 class="card-title text-3xl font-black mb-2">Create a Group</h2>
        <p class="text-sm opacity-60 mb-8 text-balance">Groups allow you to share saves with friends. Give your group a unique name to get started.</p>

        <div class="bg-base-300 p-4 rounded-xl mb-8">
          <div class="flex justify-between items-center mb-2">
            <span class="text-xs font-bold uppercase opacity-50">Owned Groups Limit</span>
            <span class="text-xs font-bold">{{ ownedGroups.length }} / 4</span>
          </div>
          <progress
              class="progress progress-primary w-full"
              :value="ownedGroups.length"
              max="4"
          ></progress>
          <p v-if="ownedGroups.length >= 4" class="text-[10px] text-error mt-2 font-bold uppercase tracking-wide">
            Limit reached. Delete an old group to create a new one.
          </p>
        </div>

        <form @submit.prevent="handleCreateGroup" class="space-y-6">
          <div v-if="err" class="alert alert-error shadow-sm text-sm">
            <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
            <span>{{err}}</span>
          </div>

          <div class="form-control w-full">
            <label class="label pt-0">
              <span class="label-text font-bold">Group Name</span>
              <span class="label-text-alt opacity-50">{{ groupName.length }}/20</span>
            </label>
            <input
                v-model="groupName"
                type="text"
                class="input input-bordered focus:input-primary w-full text-lg"
                placeholder="e.g. Weekend Warriors"
                maxlength="20"
                :disabled="ownedGroups.length >= 4"
            />
          </div>

          <div class="card-actions justify-center pt-4">
            <button
                :disabled="ownedGroups.length >= 4 || groupName.length < 3 || sentLoading"
                type="submit"
                class="btn btn-primary btn-block shadow-lg"
            >
              <span v-if="sentLoading" class="loading loading-dots"></span>
              <span v-else>Initialize Group</span>
            </button>

            <RouterLink :to="{ name: 'groups' }" class="btn btn-ghost btn-sm mt-2">
              Cancel and return
            </RouterLink>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
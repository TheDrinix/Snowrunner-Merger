<script setup lang="ts">
import Icon from "@/components/icon.vue";
import {useUserStore} from "@/stores/userStore";
import {computed} from "vue";
import {breakpointsTailwind, useBreakpoints} from "@vueuse/core";
import {useLogout} from "@/composables/useLogout";

const userStore = useUserStore();
const breakpoints = useBreakpoints(breakpointsTailwind);
const logout = useLogout();

const smAndSmaller = breakpoints.smallerOrEqual('md');

const user = computed(() => userStore.user);
</script>

<template>
  <div class="dropdown dropdown-end">
    <div
        tabindex="0"
        role="button"
        class="btn btn-ghost border border-base-300 bg-base-200/50 flex items-center gap-2 px-3 hover:border-primary/50"
    >
      <div class="avatar placeholder">
        <div class="bg-neutral text-neutral-content rounded-full w-8">
          <span class="text-xs">{{ user?.username?.charAt(0).toUpperCase() }}</span>
        </div>
      </div>
      <span class="hidden md:block font-bold text-sm">{{ user?.username }}</span>
      <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 opacity-50" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd" />
      </svg>
    </div>

    <ul
        tabindex="0"
        class="menu dropdown-content bg-base-100 rounded-2xl z-[1] mt-4 w-60 p-2 shadow-2xl border border-base-300 animate-in fade-in slide-in-from-top-2"
    >
      <template v-if="smAndSmaller">
        <li class="menu-title">Navigation</li>
        <li>
          <RouterLink :to="{ name: 'groups' }">
            <Icon name="groups" /> <span>Groups</span>
          </RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'group-create' }">
            <Icon name="add_circle" /> <span>Create a group</span>
          </RouterLink>
        </li>
        <div class="divider my-1 opacity-50"></div>
      </template>

      <li class="menu-title">Settings</li>
      <li>
        <RouterLink :to="{ name: 'account' }">
          <Icon name="account_circle" />
          <span>Account Settings</span>
        </RouterLink>
      </li>
      <li>
        <button @click="logout" class="text-error hover:bg-error/10">
          <Icon name="logout" />
          <span>Logout</span>
        </button>
      </li>
    </ul>
  </div>
</template>

<style scoped>

</style>
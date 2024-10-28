<script setup lang="ts">
import Icon from "@/components/icon.vue";
import {useUserStore} from "@/stores/userStore";
import {computed} from "vue";
import {breakpointsTailwind, useBreakpoints} from "@vueuse/core";
import {useLogout} from "@/composables/useLogout";

const userStore = useUserStore();
const breakpoints = useBreakpoints(breakpointsTailwind);
const logout = useLogout();

const smAndSmaller = breakpoints.smallerOrEqual('sm');

const user = computed(() => userStore.user);
</script>

<template>
  <div class="dropdown dropdown-end">
    <div tabindex="0" role="button" class="btn btn-ghost">
      <Icon name="person" />
      {{ user?.username }}
    </div>
    <ul tabindex="0" class="menu dropdown-content bg-base-200 rounded-box z-[1] mt-4 min-w-52 p-2 shadow">
      <template v-if="smAndSmaller">
        <li>
          <RouterLink :to="{ name: 'groups' }" exact-active-class="active">Groups</RouterLink>
        </li>
        <li>
          <RouterLink :to="{ name: 'group-create' }" exact-active-class="active">Create a group</RouterLink>
        </li>
        <div class="divider my-2" />
      </template>
      <li>
        <RouterLink :to="{ name: 'account' }" exact-active-class="active">
          <Icon name="account_circle" />
          Account
        </RouterLink>
      </li>
      <li>
        <button @click="logout">
          <Icon name="logout" />
          Logout
        </button>
      </li>
    </ul>
  </div>
</template>

<style scoped>

</style>
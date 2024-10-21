<script setup lang="ts">
import Icon from "@/components/icon.vue";
import {useUserStore} from "@/stores/userStore";
import {computed} from "vue";
import {breakpointsTailwind, useBreakpoints} from "@vueuse/core";
import {useHttp} from "@/composables/useHttp";
import {useRouter} from "vue-router";

const userStore = useUserStore();
const breakpoints = useBreakpoints(breakpointsTailwind);
const http = useHttp();
const router = useRouter();

const smAndSmaller = breakpoints.smallerOrEqual('sm');

const user = computed(() => userStore.user);

const handleLogout = async () => {
  try {
    await http.post('/auth/logout', {}, {
      withCredentials: true
    });
  } catch (e) {
    console.error(e);
  }

  userStore.logout();

  router.push({name: 'home'});
}
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
        <button>
          <Icon name="account_circle" />
          Account
        </button>
      </li>
      <li>
        <button @click="handleLogout">
          <Icon name="logout" />
          Logout
        </button>
      </li>
    </ul>
  </div>
</template>

<style scoped>

</style>
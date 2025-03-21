<script setup lang="ts">
import {useUserStore} from "@/stores/userStore";
import {computed} from "vue";
import {breakpointsTailwind, useBreakpoints} from "@vueuse/core";
import HeaderDropdown from "@/components/HeaderDropdown.vue";

const userStore = useUserStore();
const breakpoints = useBreakpoints(breakpointsTailwind);

const smAndSmaller = breakpoints.smallerOrEqual('sm');

const isAuthenticated = computed(() => userStore.isAuthenticated);
</script>

<template>
  <header>
    <nav class="navbar bg-base-200">
      <div class="container mx-auto">
        <div class="flex flex-1 items-center">
          <RouterLink class="text-primary text-lg font-medium" :to="{ name: 'home'}">Snowrunner merger</RouterLink>
          <div class="inline-flex items-center" v-if="isAuthenticated && !smAndSmaller">
            <div>
              <div class="h-8 divider divider-horizontal mr-1" />
            </div>
            <ul class="px-0 menu menu-horizontal bg-base-200 rounded-box">
              <li>
                <RouterLink :to="{ name: 'groups' }" exact-active-class="active">Groups</RouterLink>
              </li>
              <li>
                <RouterLink :to="{ name: 'group-create' }" exact-active-class="active">Create a group</RouterLink>
              </li>
            </ul>
          </div>
        </div>
        <div class="flex-none">
          <div v-if="isAuthenticated">
            <HeaderDropdown />
          </div>
          <div v-else>
            <RouterLink class="btn btn-ghost" :to="{ name: 'login' }">Sign in</RouterLink>
          </div>
        </div>
      </div>
    </nav>
  </header>
</template>

<style scoped>

</style>
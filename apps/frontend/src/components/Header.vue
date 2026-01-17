<script setup lang="ts">
import {useUserStore} from "@/stores/userStore";
import {computed} from "vue";
import {breakpointsTailwind, useBreakpoints} from "@vueuse/core";
import HeaderDropdown from "@/components/HeaderDropdown.vue";

const userStore = useUserStore();
const breakpoints = useBreakpoints(breakpointsTailwind);

const smAndSmaller = breakpoints.smallerOrEqual('md');

const isAuthenticated = computed(() => userStore.isAuthenticated);
</script>

<template>
  <header class="sticky top-0 z-50 w-full border-b border-base-300 bg-base-100/80 backdrop-blur-md">
    <nav class="navbar container mx-auto px-4 min-h-[4rem]">
      <div class="flex-1">
        <RouterLink
            :to="{ name: 'home'}"
            class="flex items-center gap-2 group transition-all"
        >
          <div class="bg-primary text-primary-content p-1.5 rounded-lg group-hover:rotate-3 transition-transform">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
            </svg>
          </div>
          <span class="text-xl font-black uppercase tracking-tighter">
          Snowrunner<span class="text-primary">Merger</span>
        </span>
        </RouterLink>

        <div v-if="isAuthenticated && !smAndSmaller" class="hidden md:flex items-center ml-6">
          <div class="h-6 w-[1px] bg-base-300 mx-4"></div>
          <ul class="menu menu-horizontal gap-2 p-0">
            <li>
              <RouterLink
                  :to="{ name: 'groups' }"
                  active-class="bg-primary/10 text-primary font-bold"
                  class="rounded-lg transition-all"
              >
                Groups
              </RouterLink>
            </li>
            <li>
              <RouterLink
                  :to="{ name: 'group-create' }"
                  active-class="bg-primary/10 text-primary font-bold"
                  class="rounded-lg transition-all"
              >
                Create Group
              </RouterLink>
            </li>
          </ul>
        </div>
      </div>

      <div class="flex-none gap-2">
        <div v-if="isAuthenticated">
          <HeaderDropdown />
        </div>
        <div v-else>
          <RouterLink class="btn btn-primary btn-sm md:btn-md shadow-md" :to="{ name: 'login' }">
            Sign in
          </RouterLink>
        </div>
      </div>
    </nav>
  </header>
</template>

<style scoped>

</style>
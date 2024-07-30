<script setup lang="ts">
import {useUserStore} from "@/stores/userStore";
import {computed} from "vue";
import Icon from "@/components/icon.vue";

const userStore = useUserStore();

const isAuthenticated = computed(() => userStore.isAuthenticated);

const user = computed(() => userStore.user);
</script>

<template>
  <header>
    <nav class="navbar bg-base-200">
      <div class="container mx-auto">
        <div class="flex flex-1 items-center">
          <RouterLink :to="{ name: 'home'}">Snowrunner merger</RouterLink>
          <div>
            <div class="h-8 divider divider-horizontal mr-1" />
          </div>
          <div class="inline-flex items-center" v-if="isAuthenticated">
            <ul class="px-0 menu menu-horizontal bg-base-200 rounded-box">
              <li>
                <RouterLink :to="{ name: 'groups' }" exact-active-class="active">Groups</RouterLink>
              </li>
              <li>
                <RouterLink :to="{ name: 'groups' }">Groups</RouterLink>
              </li>
            </ul>
          </div>
        </div>
        <div class="flex-none">
          <div v-if="isAuthenticated">
            <div class="dropdown dropdown-end">
              <div tabindex="0" role="button" class="btn btn-ghost">
                <Icon name="person" />
                {{ user?.username }}
              </div>
              <ul tabindex="0" class="menu dropdown-content bg-base-100 rounded-box z-[1] mt-4 min-w-52 p-2 shadow">
                <li>
                  <button>
                    <Icon name="account_circle" />
                    Account
                  </button>
                </li>
                <li>
                  <button>
                    <Icon name="logout" />
                    Logout
                  </button>
                </li>
              </ul>
            </div>
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
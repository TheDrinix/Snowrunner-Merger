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
        <div class="flex-1">
          <RouterLink :to="{ name: 'home'}">Snowrunner merger</RouterLink>
          <div class="inline-flex" v-if="isAuthenticated">
            <span class="px-2 text-2xl">|</span>
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
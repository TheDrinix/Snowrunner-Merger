<script setup lang="ts">
import {computed, onBeforeUnmount, ref} from "vue";
import {useRoute, useRouter} from "vue-router";
import {useHttp} from "@/composables/useHttp";
import {useDelayedRedirect} from "@/composables/useDelayedRedirect";

const route = useRoute();
const router = useRouter();
const http = useHttp();
const { timer, startRedirect, stopRedirect } = useDelayedRedirect('login', 10);

const loading = ref(true);
const err = ref(false);

const userId = computed((): string => {
  return route.query['user-id']?.toString() ?? '';
})

const token = computed((): string => {
  return route.query['token']?.toString() ?? '';
})

if (!userId.value || !token.value) {
  startRedirect();
  err.value = true;
}

http.post('/auth/verify-email', {
  userId: userId.value,
  token: token.value
}).then(() => {
  loading.value = false;
  err.value = false;

  startRedirect();
}).catch(() => {
    err.value = true;
    loading.value = false;

    startRedirect();
})

onBeforeUnmount(() => {
  stopRedirect();
})
</script>

<template>
  <div class="max-w-md mx-auto py-20 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300">
      <div class="card-body text-center p-10">

        <template v-if="loading">
          <div class="flex flex-col items-center gap-6">
            <span class="loading loading-ring w-20 h-20 text-primary"></span>
            <div>
              <h3 class="text-2xl font-black">Verifying...</h3>
              <p class="opacity-60">Synchronizing with the Snowrunner servers</p>
            </div>
          </div>
        </template>

        <template v-else-if="err">
          <div class="w-20 h-20 bg-error/10 text-error rounded-full flex items-center justify-center mx-auto mb-6">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M6 18L18 6M6 6l12 12" /></svg>
          </div>
          <h3 class="text-2xl font-black text-error">Link Expired</h3>
          <p class="opacity-70 mt-2 mb-6 text-sm">The confirmation link is either invalid or has timed out.</p>

          <div class="badge badge-outline gap-2 p-4 mx-auto">
            Redirecting to Login in <span class="countdown font-mono">{{timer}}</span>s
          </div>
        </template>

        <template v-else>
          <div class="w-20 h-20 bg-success/10 text-success rounded-full flex items-center justify-center mx-auto mb-6 shadow-inner">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" /></svg>
          </div>
          <h3 class="text-2xl font-black text-success">Verified!</h3>
          <p class="opacity-70 mt-2 mb-6">Your engine is primed and your account is ready to go.</p>

          <RouterLink :to="{ name: 'login' }" class="btn btn-primary btn-block">
            Login Now ({{timer}}s)
          </RouterLink>
        </template>

      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
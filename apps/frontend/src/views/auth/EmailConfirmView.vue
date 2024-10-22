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
  return route.query['user-id'] ?? '';
})

const token = computed((): string => {
  return route.query['token'] ?? '';
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

    startRedirect();
}).catch(() => {
    err.value = false;
    loading.value = false;

    startRedirect();
})

err.value = false;

onBeforeUnmount(() => {
  stopRedirect();
})
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      <span class="card-title">Email confirmation</span>
    </div>
    <div class="card-body pt-4">
      <template v-if="loading">
        <div class="flex flex-col items-center">
          <h3 class="text-lg font-bold">Please wait while we verify your email</h3>
          <span class="loading loading-bars loading-lg mt-2"></span>
        </div>
      </template>
      <template v-else-if="err">
        <div class="alert alert-error flex flex-col items-center gap-0">
            <p class="text-lg">The confirmation link is either invalid or expired</p>
            <p>Request a new link by trying to log in</p>
            <p>You'll be redirected to login page in {{timer}} seconds</p>
        </div>
      </template>
      <template v-else>
        <div>
          <h3 class="text-lg font-bold">Your email has been verified successfully</h3>
          <p>You'll be able to login now</p>
          <p>You'll be redirected to login page in {{timer}} seconds</p>
        </div>
      </template>
    </div>
  </div>
</template>

<style scoped>

</style>
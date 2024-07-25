<script setup lang="ts">
import {onBeforeMount} from "vue";
import {useRoute, useRouter} from "vue-router";
import {useHttp} from "@/composables/useHttp";
import type {LoginResponse} from "@/types/auth";
import {useUserStore} from "@/stores/userStore";

const route = useRoute();
const router = useRouter();
const http = useHttp();
const userStore = useUserStore();

onBeforeMount(async () => {
  const { code, state } = route.query;

  if (!code || !state) {
    await router.push({ name: 'login' });
  }

  console.log('Google sign-in successful');

  // Exchange the Google code for an access token
  const res = await http.get<LoginResponse>('/auth/google/signin/callback', {
    params: {
      code,
      state
    },
    withCredentials: true
  });

  if (res.status < 300) {
    console.log('User logged in successfully');
    userStore.signIn(res.data);
  } else {
    console.error('Failed to log in user');
  }

  await router.push({ name: 'home' });
});
</script>

<template>
  <div class="flex items-center flex-col">
    <h3 class="text-xl mb-4">Signing in</h3>
    <progress class="progress w-4/5 sm:w-2/3 lg:w-1/2" />
  </div>
</template>

<style scoped>

</style>
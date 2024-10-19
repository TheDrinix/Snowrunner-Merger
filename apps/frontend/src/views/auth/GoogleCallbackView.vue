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
  try {
    const res = await http.get<LoginResponse>('/auth/google/signin/callback', {
      params: {
        code,
        state
      },
      withCredentials: true
    });

    userStore.signIn(res.data);
  } catch (e) {
    console.error('Failed to exchange Google code for access token');
    await router.push({ name: 'login', query: { error: 'There was an error trying to sign you in. Please try again later or try signing in using you email and password' } });
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
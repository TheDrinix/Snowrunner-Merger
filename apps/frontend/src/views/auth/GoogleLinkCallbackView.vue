<script setup lang="ts">
import {useUserStore} from "@/stores/userStore";
import {useToaster} from "@/stores/toastStore";
import {useRoute, useRouter} from "vue-router";
import {onBeforeMount} from "vue";
import {useHttp} from "@/composables/useHttp";
import type {User} from "@/types/auth";

const router = useRouter();
const route = useRoute();
const userStore = useUserStore();
const { createToast } = useToaster();
const http = useHttp();

onBeforeMount(async () => {
  if (!userStore.isAuthenticated) {
    await userStore.refreshToken();

    if (!userStore.isAuthenticated) {
      createToast('You must be logged in to access this page', 'error');
      await router.push({ name: 'login' });
      return;
    }
  }

  const { code, state } = route.query;
  if (!code || !state) {
    createToast('Google account linking failed, please try again later.', 'error');
    await router.push({ name: 'account' });
    return;
  }

  try {
    let redirectUrl = new URL('/auth/google/link', window.location.origin);

    const res = await http.get<User>('/auth/google/link/callback', {
      params: {
        code,
        state,
        callbackUrl: redirectUrl.toString(),
      },
      withCredentials: true
    });

    userStore.storeUser(res.data);
    createToast('Google account linked successfully', 'success');
    await router.push({ name: 'account' });
  } catch (e: any) {
    console.error(e);
    if (e.response?.status === 401) {
      createToast('You have to be logged in to link a google account.', 'error');
      await router.push({ name: 'login' });
    } else if (e.response?.status === 409) {
      createToast('Google account is already linked to another user.', 'error');
      await router.push({ name: 'account' });
    } else {
      createToast('Google account linking failed, please try again later.', 'error');
      await router.push({ name: 'account' });
    }
  }
})
</script>

<template>
  <div class="flex items-center justify-center min-h-[calc(100vh-16rem)] flex-col">
    <span class="loading loading-spinner w-72"></span>
    <p class="text-xl font-bold mt-4">Linking your Google account...</p>
  </div>
</template>

<style scoped>

</style>
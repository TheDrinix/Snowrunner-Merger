<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {useToaster} from "@/stores/toastStore";
import {computed, watch} from "vue";
import {useHttp} from "@/composables/useHttp";
import type {LoginResponse} from "@/types/auth";
import {useUserStore} from "@/stores/userStore";
import {AxiosError} from "axios";

const route = useRoute();
const router = useRouter();
const { createToast } = useToaster();
const http = useHttp();
const userStore = useUserStore();

console.log("Hello from LinkAccountView");

if (!route.query.token || !route.query.email) {
  createToast('Invalid linking token', 'error');
  router.push({ name: 'login' });
}

console.log("Linking token received");

const linkingToken = computed(() => {
  return route.query.token as string;
})

const email = computed(() => {
  return route.query.email as string;
})

watch([linkingToken, email], () => {
  if (!route.query.token || !route.query.email) {
    createToast('Invalid linking token', 'error');
    router.push({ name: 'login' });
  }
})

console.log("Linking token received 2");

const handleLinkAccount = async () => {
  try {
    const res = await http.post<LoginResponse>('/auth/google/link-account', {
      linkingToken: linkingToken.value,
    }, {
      withCredentials: true
    });

    createToast('Account linked successfully', 'success');

    userStore.signIn(res.data);
    await router.push({ name: 'groups' });
  } catch (e) {
    if (e instanceof AxiosError) {
      if (e.response?.status === 401) {
        createToast('Invalid or expired linking token', 'error');
        return await router.push({ name: 'login' });
      }
    }

    createToast('Failed to link account, please try again later', 'error');
    await router.push({ name: 'login' });
  }
}

const handleCancel = () => {
  router.push({ name: 'login' });
}

console.log("End of LinkAccountView");
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      <h3 class="text-lg font-medium">Link your google account</h3>
    </div>
    <div class="card-body pt-4">
      <div class="flex flex-col gap-2">
        <p>There's already an account created using the same email <span>({{email}})</span> as your google account.</p>
        <p>Do you want to link your google account to this account?</p>
      </div>
    </div>
    <div class="card-actions p-4">
      <div class="flex justify-end w-full">
        <div class="join">
          <button class="btn btn-sm btn-primary join-item" @click="handleLinkAccount">Link account</button>
          <button class="btn btn-sm btn-secondary join-item" @click="handleCancel">Cancel</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
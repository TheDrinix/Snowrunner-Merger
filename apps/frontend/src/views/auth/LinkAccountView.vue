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

const provider = computed(() => {
  return route.params.provider as string;
});

if (!route.query.token || !route.query.email) {
  createToast('Invalid linking token', 'Your linking token is missing', 'error');
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
    createToast('Invalid linking token', 'Your linking token is missing', 'error');
    router.push({ name: 'login' });
  }
})

const handleLinkAccount = async () => {
  try {
    const res = await http.post<LoginResponse>(`/auth/${provider.value}/link-account`, {
      linkingToken: linkingToken.value,
    }, {
      withCredentials: true
    });

    createToast('Account linked successfully', `Your ${provider.value} account has been linked to your account.`, 'success');

    userStore.signIn(res.data);
    router.push({ name: 'groups' });
    
  } catch (e) {
    if (e instanceof AxiosError) {
      if (e.response?.status === 401) {
        createToast('Invalid or expired linking token', '', 'error');
        return router.push({ name: 'login' });
      } else if (e.response?.status === 409) {
        createToast('Accounts already linked', `Your ${provider.value} account is already linked to another account. Please login using that account.`, 'error');
        return router.push({ name: 'login' });
      } else if (e.response?.status === 400) {
        createToast('Linking failed', e.response.data.message || 'Failed to link accounts, please try again later', 'error');
        return router.push({ name: 'login' });
      }
    }

    createToast('Account linking failed', `Failed to link ${provider.value} account, please try again later`, 'error');
    router.push({ name: 'login' });
  }
} 

const handleCancel = () => {
  router.push({ name: 'login' });
}
</script>

<template>
  <div class="max-w-lg mx-auto my-12 px-4">
    <div class="card bg-base-200 shadow-2xl border border-warning/30">
      <div class="card-body p-8 text-center">
        <div class="flex items-center justify-center gap-4 mb-6">
          <div class="w-16 h-16 rounded-full bg-base-300 flex items-center justify-center shadow-inner">
            <Icon name="person" class="w-8 h-8 opacity-40" />
          </div>
          <div class="flex flex-col items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-warning animate-pulse" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
            </svg>
          </div>
          <div class="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center border border-primary/20">
            <img class="w-8 h-8" :src="`/${provider}.svg`" :alt="provider" />
          </div>
        </div>

        <h3 class="text-2xl font-black uppercase tracking-tighter mb-2">Sync Detected</h3>
        <p class="text-sm opacity-80 leading-relaxed mb-6">
          The email <span class="badge badge-ghost font-mono text-xs">{{email}}</span> is already registered.
          Would you like to link your <span class="font-bold capitalize">{{provider}}</span> account to your existing profile?
        </p>

        <div class="flex flex-col gap-2">
          <button class="btn btn-primary btn-block shadow-lg" @click="handleLinkAccount">
            Yes, Link Accounts
          </button>
          <button class="btn btn-ghost btn-sm opacity-50" @click="handleCancel">
            Cancel and return
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
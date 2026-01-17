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

    createToast('Account linked successfully', `Your ${provider} account has been linked to your account.`, 'success');

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

    createToast(`Failed to link ${provider.value} account, please try again later`, 'error');
    router.push({ name: 'login' });
  }
} 

const handleCancel = () => {
  router.push({ name: 'login' });
}
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      <h3 class="text-lg font-medium">Link your {{provider}} account</h3>
    </div>
    <div class="card-body pt-4">
      <div class="flex flex-col gap-2">
        <p>There's already an account created using the same email <span>({{email}})</span> as your {{ provider }} account.</p>
        <p>Do you want to link your {{ provider }} account to this account?</p>
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
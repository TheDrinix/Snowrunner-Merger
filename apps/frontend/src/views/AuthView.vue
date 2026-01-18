<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, onMounted} from "vue";
import {useHttp} from "@/composables/useHttp";
import { useToaster } from "@/stores/toastStore";
import { useUserStore } from "@/stores/userStore";
import {useOAuthStore} from "@/stores/oauthStore";

const route = useRoute();
const router = useRouter();
const toast = useToaster();
const oauthStore = useOAuthStore();

const http = useHttp();

if (route.name == 'auth') {
  router.push({name: 'login'});
}

const title = computed(() => {
  return route.name == 'login' ? 'Sign in to an existing account' : 'Create a new account'
})

const oauthProviders = computed(() => oauthStore.oauthProviders);

const handleOauthSignIn = async (provider: string) => {
  try {
    let redirectUrl = new URL(`/auth/${provider}/callback`, window.location.origin);

    const oauthSignInUrl = await http.get<string>(`/auth/${provider}/signin/`, {
      withCredentials: true,
      params: {
        callbackUrl: redirectUrl.toString(),
      }
    });

    // Redirect the user to the OAuth provider's sign-in page
    window.location.href = oauthSignInUrl.data;
  } catch (e) {
    console.error(`Failed to get ${provider} sign-in URL`);
    toast.createToast(`Failed to get ${provider} sign-in URL`, 'error');
  }
}

onMounted(() => {
  if (!oauthProviders.value.length) {
    oauthStore.fetchOAuthProviders();
  }
})
</script>

<template>
  <div class="max-w-md mx-auto my-12 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300 overflow-hidden">
      <div class="bg-base-300/50 p-6 pb-0">
        <h2 class="text-xs font-black uppercase tracking-[0.2em] text-primary mb-4 text-center">{{ title }}</h2>

        <div class="tabs tabs-boxed bg-base-300 p-1 mb-4">
          <RouterLink
              :to="{ name: 'login' }"
              class="tab flex-1 font-bold transition-all"
              active-class="tab-active !bg-primary !text-primary-content"
          >Login</RouterLink>
          <RouterLink
              :to="{ name: 'register' }"
              class="tab flex-1 font-bold transition-all"
              active-class="tab-active !bg-primary !text-primary-content"
          >Register</RouterLink>
        </div>
      </div>

      <div class="card-body pt-4">
        <RouterView />

        <div class="divider text-xs uppercase opacity-40 font-bold my-6">Or continue with</div>

        <div class="grid grid-cols-2 gap-3">
          <button
              v-for="provider in oauthProviders"
              :key="provider"
              class="btn btn-outline border-base-300 gap-2 font-bold capitalize"
              @click="() => handleOauthSignIn(provider)"
          >
            <img class="w-5 h-5" :src="`/${provider}.svg`" :alt="`${provider} icon`" />
            {{ provider }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
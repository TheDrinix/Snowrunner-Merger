<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, onMounted} from "vue";
import {useHttp} from "@/composables/useHttp";
import { useToaster } from "@/stores/toastStore";
import { useUserStore } from "@/stores/userStore";

const route = useRoute();
const router = useRouter();
const toast = useToaster();
const userStore = useUserStore();

const http = useHttp();

if (route.name == 'auth') {
  router.push({name: 'login'});
}

const title = computed(() => {
  return route.name == 'login' ? 'Sign in to existing account' : 'Create a new account'
})

const oauthProviders = computed(() => userStore.oauthProviders);

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
  if (!oauthProviders.value) {
    userStore.fetchOAuthProviders();
  }
})
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      {{ title }}
    </div>
    <div class="card-body pt-4">
      <RouterView />
      <hr class="divider border-none my-2" />
      <div>
        <h3>Sign-in using an external service:</h3>
        <div class="mt-2 flex flex-wrap gap-1">
          <button 
            v-for="provider in oauthProviders" 
            :key="provider"
            class="btn btn-ghost gap-0"
            @click="() => handleOauthSignIn(provider)"
          >
            <div class="md:p-1">
              <img class="w-8 h-8 md:w-6 md:h-6" :src="`/${provider}.svg`" :alt="`${provider} icon`" />
            </div>
            <span class="hidden md:inline-block capitalize">{{ provider }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
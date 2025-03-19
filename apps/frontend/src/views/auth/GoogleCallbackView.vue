<script setup lang="ts">
import {onBeforeMount} from "vue";
import {useRoute, useRouter} from "vue-router";
import {useHttp} from "@/composables/useHttp";
import {type GoogleLoginRes} from "@/types/auth";
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
    let redirectUrl = new URL('/auth/google/callback', window.location.origin);

    const res = await http.get<GoogleLoginRes>('/auth/google/signin/callback', {
      params: {
        code,
        state,
        callbackUrl: redirectUrl.toString(),
      },
      withCredentials: true
    });

    console.log(res.data);

    // TokenType = ACCESS_TOKEN
    if (res.data.tokenType === 1) {
      userStore.signIn(res.data.data);

      await router.push({ name: 'groups' });
    }
    // TokenType = LINKING_TOKEN
    else if (res.data.tokenType === 2) {
      const linkingToken = res.data.data.token;
      const email = res.data.data.user.email;

      console.log("Linking token received, redirecting to link account view");

      await router.push({ name: 'link-google-account', query: {
        token: linkingToken,
        email: email,
      }});
    }
    // TokenType = COMPLETION_TOKEN
    else if (res.data.tokenType === 4) {
      const completionToken = res.data.data.token;
      const email = res.data.data.email;

      await router.push({ name: 'complete-google-account', query: {
        token: completionToken,
        email: email,
      }});
    }


  } catch (e) {
    console.error('Failed to exchange Google code for access token');
    await router.push({ name: 'login', query: { error: 'There was an error trying to sign you in. Please try again later or try signing in using you email and password' } });
  }
});
</script>

<template>
  <div class="flex items-center justify-center min-h-[calc(100vh-16rem)] flex-col">
    <span class="loading loading-spinner w-72"></span>
    <p class="text-xl font-bold mt-4">Signing you in...</p>
  </div>
</template>

<style scoped>

</style>
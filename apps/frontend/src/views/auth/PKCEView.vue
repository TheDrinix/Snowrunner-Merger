<script setup lang="ts">
import { useHttp } from "@/composables/useHttp";
import { useToaster } from "@/stores/toastStore";
import { onBeforeMount } from "vue";
import { useRoute, useRouter } from "vue-router";

const route = useRoute();
const router = useRouter();
const http = useHttp();
const toaster = useToaster();

onBeforeMount(async () => {
  const { challenge_code, redirect_uri, client_id } = route.query;
  if (!challenge_code) {
    router.push({ name: "home" });
  }

  try {
    const res = await http.get("oauth/authorize", {
      params: {
        codeChallenge: challenge_code,
        redirectUri: redirect_uri,
        clientId: client_id,
        responseType: "code",
      },
      withCredentials: true,
    });

    const redirectUrl = new URL(res.data);
    window.location.href = redirectUrl.toString();
  } catch (error) {
    toaster.createToast(
      "OAuth Authorization Failed",
      "An error occurred during OAuth authorization. Please try again.",
      "error",
    );
    router.push({ name: "home" });
  }
});
</script>

<template>
  <div class="flex items-center justify-center min-h-[60vh] px-6">
    <div class="text-center">
      <div class="relative flex items-center justify-center mb-8">
        <span
          class="loading loading-ring w-32 h-32 text-primary opacity-20"
        ></span>
        <span
          class="loading loading-ring w-24 h-24 text-primary absolute opacity-40"
        ></span>
        <span
          class="loading loading-ring w-16 h-16 text-primary absolute"
        ></span>
      </div>

      <h2 class="text-2xl font-black uppercase tracking-widest animate-pulse">
        Authenticating<span class="text-primary">...</span>
      </h2>
      <p class="text-xs font-mono opacity-50 mt-2 uppercase tracking-[0.2em]">
        Securing Authorization Codes
      </p>
    </div>
  </div>
</template>

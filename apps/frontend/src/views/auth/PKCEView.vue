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
  const { code_challenge, redirect_uri, client_id, scope, state } = route.query;
  if (!code_challenge) {
    router.push({ name: "home" });
  }

  let redirectUrl: string;
  try {
    const res = await http.get("auth/oauth/authorize", {
      params: {
        codeChallenge: code_challenge,
        redirectUri: redirect_uri,
        clientId: client_id,
        scope,
        state,
        responseType: "code",
      },
      withCredentials: true,
    });

    redirectUrl = res.data;
  } catch (error) {
    console.log("Error during auth code request");
    console.log(error);

    toaster.createToast(
      "OAuth Authorization Failed",
      "An error occurred during OAuth authorization. Please try again.",
      "error",
    );
    router.push({ name: "home" });
    return;
  }

  try {
    const res = await fetch(redirectUrl, {
      method: "GET",
    });

    toaster.createToast(
      "OAuth Authorization Successful",
      "You have been successfully authorized. You can now return to the application.",
      "success",
    );
    router.push({ name: "home" });
  } catch (error) {
    console.log("Error during delivering auth code");
    console.log(error);

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

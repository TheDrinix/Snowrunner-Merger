<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed} from "vue";
import {useHttp} from "@/composables/useHttp";
import {useUserStore} from "@/stores/userStore";

const route = useRoute();
const router = useRouter();

const http = useHttp();
const userStore = useUserStore();

if (route.name == 'auth') {
  router.push({name: 'login'});
}

const title = computed(() => {
  return route.name == 'login' ? 'Sign in to existing account' : 'Create a new account'
})

const handleGoogleSignIn = async () => {
  const googleSignInUrl = await http.get<string>('/auth/google/signin', {
    withCredentials: true
  });

  if (googleSignInUrl.status != 200) {
    console.error('Failed to get Google sign-in URL');
    return;
  }

  // Redirect the user to the Google sign-in page
  window.location.href = googleSignInUrl.data;
}
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
        <div class="mt-2">
          <button @click="handleGoogleSignIn" class="btn btn-outline gap-0">
            <img class="w-8 h-8" src="/google.svg" alt="Google icon">
            Google
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
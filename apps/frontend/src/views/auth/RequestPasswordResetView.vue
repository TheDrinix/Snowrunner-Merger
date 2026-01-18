<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {ref, watch} from "vue";
import {useRoute, useRouter} from "vue-router";
import Icon from "@/components/icon.vue";
import {useHttp} from "@/composables/useHttp";
import {useToaster} from "@/stores/toastStore";

const route = useRoute();
const router = useRouter();
const http = useHttp();
const { createToast } = useToaster();

const email = ref(route.query['email']?.toString() ?? "");
watch(() => route.query.email, (value) => {
  email.value = value?.toString() ?? "";
});

const isLoading = ref(false);
const error = ref("");

const handlePasswordReset = async () => {
  isLoading.value = true;

  try {
    await http.post("/auth/request-password-reset", {
      email: email.value
    });

    await router.push({name: "reset-password-confirm"});
  } catch (e: any) {
    if (e.response.data.title) {
      createToast(e.response.data.title, '', 'error');
    }
  }

  isLoading.value = false;
}
</script>

<template>
  <div class="max-w-md mx-auto my-12 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300 overflow-hidden">
      <div class="card-body p-8">
        <div class="text-center mb-6">
          <div class="w-16 h-16 bg-primary/10 text-primary rounded-2xl flex items-center justify-center mx-auto mb-4 border border-primary/20">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
            </svg>
          </div>
          <h2 class="text-3xl font-black uppercase tracking-tighter">Reset Password</h2>
          <p class="text-sm opacity-60 mt-1">Enter your email and we'll send you a secure recovery link.</p>
        </div>

        <form @submit.prevent="handlePasswordReset" class="space-y-6">
          <TextInput v-model="email" name="email" placeholder="Recovery Email" type="email" autocomplete="email">
            <template #icon-prepend>
              <Icon name="mail" class="opacity-50" />
            </template>
          </TextInput>

          <div class="flex flex-col gap-4">
            <button type="submit" class="btn btn-primary btn-block shadow-lg">
              Send Reset Link
            </button>
            <RouterLink :to="{ name: 'login' }" class="btn btn-ghost btn-sm opacity-50">
              Return to Sign In
            </RouterLink>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
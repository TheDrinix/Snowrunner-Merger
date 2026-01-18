<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import TextInput from "@/components/forms/TextInput.vue";
import Icon from "@/components/icon.vue";
import {useHttp} from "@/composables/useHttp";
import {useForm} from "vee-validate";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useDelayedRedirect} from "@/composables/useDelayedRedirect";

const route = useRoute();
const router = useRouter();
const http = useHttp();
const { timer, startRedirect } = useDelayedRedirect('login', 10);

const userId = computed(() => {
  return route.query['user-id'];
})

const code = computed(() => {
  return route.query['token'];
})

const err = ref('');

const isValid = computed(() => {
  return userId.value && code.value && !err.value;
})

if (!isValid.value) {
  startRedirect();
}

const { errors, defineField } = useForm({
  validationSchema: toTypedSchema(
      yup.object({
        password: yup
            .string()
            .required("Password is required")
            .min(8, "Password must be at least 8 characters")
            .matches(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}/gm, "Password must contain at least one uppercase letter, one lowercase letter, and one number"),
        confirmPassword: yup
            .string()
            .required("Confirm password is required")
            .oneOf([yup.ref("password")], "Passwords must match")
      })
  )
});

const [password, passwordAttrs] = defineField("password");
const [confirmPassword, confirmPasswordAttrs] = defineField("confirmPassword");

const loading = ref(false);
const isFormValid = computed(() => {
  return !!password.value && !!confirmPassword.value && !errors.value.password && !errors.value.confirmPassword;
})

const handlePasswordReset = async () => {
  if (!isFormValid.value) return;

  loading.value = true;

  try {
    const body = {
      userId: userId.value,
      token: code.value,
      password: password.value
    };

    await http.post('/auth/reset-password', body);

    router.push({name: 'login', query: { msg: "Your password has been reset" }});
  } catch (e) {
    err.value = "This link is either invalid or expired. Request a new password reset link."
    startRedirect();
  }

  loading.value = false;
}
</script>

<template>
  <div class="max-w-md mx-auto my-12 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300">
      <div class="card-body p-8">
        <template v-if="isValid">
          <div class="text-center mb-8">
            <h2 class="text-3xl font-black uppercase tracking-tighter">Set New Password</h2>
            <p class="text-sm opacity-60">Finalize your security override. Make it strong!</p>
          </div>

          <form @submit.prevent="handlePasswordReset" class="space-y-4">
            <TextInput v-model="password" name="password" placeholder="New Password" type="password" autocomplete="new-password" :error="errors.password">
              <template #icon-prepend><Icon name="lock" class="opacity-50" /></template>
            </TextInput>

            <TextInput v-model="confirmPassword" name="confirmPassword" placeholder="Confirm New Password" type="password" autocomplete="new-password" :error="errors.confirmPassword">
              <template #icon-prepend><Icon name="lock" class="opacity-50" /></template>
            </TextInput>

            <button :disabled="!isFormValid || loading" type="submit" class="btn btn-primary btn-block shadow-lg mt-4">
              <span v-if="loading" class="loading loading-dots"></span>
              <span v-else>Update Password</span>
            </button>
          </form>
        </template>

        <template v-else>
          <div class="text-center py-6">
            <div class="w-20 h-20 bg-error/10 text-error rounded-full flex items-center justify-center mx-auto mb-6">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
            </div>
            <h3 class="text-2xl font-black text-error uppercase tracking-tighter">Access Denied</h3>
            <p class="opacity-70 mt-2 mb-8 leading-relaxed">
              This recovery link is invalid or has timed out. For security, these links expire quickly.
            </p>

            <div class="flex flex-col gap-3">
              <RouterLink :to="{ name: 'reset-password-request' }" class="btn btn-outline btn-block">
                Request New Link
              </RouterLink>
              <div class="badge badge-ghost gap-2 p-4 mx-auto opacity-50">
                Returning to Login in <span class="countdown font-mono">{{timer}}</span>s
              </div>
            </div>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
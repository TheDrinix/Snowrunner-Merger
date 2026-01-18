<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {useForm} from "vee-validate";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {computed, ref, watch} from "vue";
import {useRoute, useRouter} from "vue-router";
import {useToaster} from "@/stores/toastStore";
import {useHttp} from "@/composables/useHttp";
import Icon from "@/components/icon.vue";
import type {LoginResponse} from "@/types/auth";
import {AxiosError} from "axios";
import {useUserStore} from "@/stores/userStore";

const route = useRoute();
const router = useRouter();
const { createToast } = useToaster();
const http = useHttp();
const userStore = useUserStore();

const provider = computed(() => {
  return route.params.provider as string;
});

if (!route.query.token || !route.query.email) {
  createToast('Invalid completion token', 'Your completion token is missing', 'error');
  router.push({ name: 'login' });
}

const completionToken = computed(() => {
  return route.query.token as string;
})

const email = computed(() => {
  return route.query.email as string;
})

watch([completionToken, email], () => {
  if (!route.query.token || !route.query.email) {
    createToast('Invalid completion token', 'Your completion token is missing', 'error');
    router.push({ name: 'login' });
  }
})

const { errors, defineField, setErrors, validate } = useForm({
  validationSchema: toTypedSchema(
    yup.object({
      username: yup
          .string()
          .required("Username is required")
          .min(3, "Username must be at least 3 characters"),
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

const [username, usernameAttrs] = defineField("username");
const [password, passwordAttrs] = defineField("password");
const [confirmPassword, confirmPasswordAttrs] = defineField("confirmPassword");

const loading = ref(false);

const handleAccountCompletion = async () => {
  const validation = await validate()
  if (!validation.valid) {
    return;
  }

  if (!yup.string().email().validateSync(email.value)) {
    createToast('Invalid email address', '', 'error');
    return;
  }

  loading.value = true;

  try {
    const res = await http.post<LoginResponse>(`/auth/${provider.value}/finish-account-setup`, {
      completionToken: completionToken.value,
      username: username.value,
      password: password.value
    });

    createToast('Account setup complete', 'Your account has been finished', 'success');

    userStore.signIn(res.data);
    router.push({ name: 'groups' });
  } catch (e) {
    if (e instanceof AxiosError && e.response?.status === 401) {
      createToast('Invalid or expired completion token', '', 'error');
      return router.push({ name: 'login' });
    } else if (e instanceof AxiosError && e.response?.status === 400) {
      createToast('Account setup failed', e.response.data.message || 'Failed to finish account setup, please try again later', 'error');
      return router.push({ name: 'login' });
    }

    createToast('Account setup failed', `Failed to finish ${provider.value} account setup, please try again later`, 'error');
    router.push({ name: 'login' });
  }
}
</script>

<template>
  <div class="max-w-md mx-auto my-12 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300">
      <div class="card-body p-8">
        <div class="text-center mb-8">
          <div class="badge badge-primary font-black mb-2 px-4 uppercase tracking-tighter">Phase 2: Identity</div>
          <h2 class="text-3xl font-black uppercase">Finalize Account</h2>
          <p class="text-sm opacity-60 mt-1">We've linked your account. Just a few more details to get your engine started.</p>
        </div>

        <form @submit.prevent="handleAccountCompletion" class="space-y-4">
          <div class="form-control">
            <h3 class="label px-1 py-1">
              <span class="label-text-alt opacity-50 font-bold uppercase tracking-widest">Verified Email</span>
            </h3>
            <div class="relative group">
              <label class="transition-all input input-bordered flex items-center gap-2">
                <Icon name="mail" class="opacity-30" />
                <input disabled v-model="email" type="text" class="w-full bg-base-300 opacity-80 cursor-not-allowed border-dashed" name="email" placeholder="Email" />
              </label>
              <div class="absolute right-4 top-3 tooltip tooltip-left" data-tip="Provided by external service">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-success opacity-50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              </div>
            </div>
          </div>

          <div class="space-y-1">
            <TextInput v-model="username" name="username" placeholder="Choose Username" autocomplete="username" :error="errors.username">
              <template #icon-prepend><Icon name="person" class="opacity-50" /></template>
            </TextInput>

            <TextInput v-model="password" name="password" placeholder="Set Password" type="password" autocomplete="password" :error="errors.password">
              <template #icon-prepend><Icon name="lock" class="opacity-50" /></template>
            </TextInput>

            <TextInput v-model="confirmPassword" name="confirmPassword" placeholder="Confirm Password" type="password" autocomplete="password" :error="errors.confirmPassword">
              <template #icon-prepend><Icon name="lock" class="opacity-50" /></template>
            </TextInput>
          </div>

          <button :disabled="loading" type="submit" class="btn btn-primary btn-block shadow-lg mt-4 h-14">
            <span v-if="loading" class="loading loading-dots"></span>
            <span v-else class="flex items-center gap-2">
            Complete Setup
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
            </svg>
          </span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
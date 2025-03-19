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

if (!route.query.token || !route.query.email) {
  createToast('Invalid linking token', 'error');
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
    createToast('Invalid linking token', 'error');
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
    createToast('Invalid email address', 'error');
    await router.push({ name: 'login' });
    return;
  }

  loading.value = true;

  try {
    const res = await http.post<LoginResponse>('/auth/google/finish-account-setup', {
      completionToken: completionToken.value,
      username: username.value,
      password: password.value
    });

    createToast('Account setup complete', 'success');

    userStore.signIn(res.data);
    await router.push({ name: 'groups' });
  } catch (e) {
    if (e instanceof AxiosError && e.response?.status === 401) {
      createToast('Invalid or expired completion token', 'error');
      return await router.push({ name: 'login' });
    }

    createToast('Failed to link account, please try again later', 'error');
    await router.push({ name: 'login' });
  }
}
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      <h3 class="text-lg font-medium">
        Finish setting up your account
      </h3>
    </div>

    <div class="card-body pt-4">
      <div class="flex flex-col">
        <form @submit.prevent="handleAccountCompletion">
          <div class="flex flex-col gap-4">
            <TextInput v-model="email" disabled name="email" placeholder="Email">
              <template #icon-prepend>
                <Icon name="mail" />
              </template>
            </TextInput>
            <TextInput v-model="username" name="username" placeholder="Username" autocomplete="username" :error="errors.username">
              <template #icon-prepend>
                <Icon name="person" />
              </template>
            </TextInput>
            <TextInput v-model="password" name="password" placeholder="Password" type="password" autocomplete="password" :error="errors.password">
              <template #icon-prepend>
                <Icon name="lock" />
              </template>
            </TextInput>
            <TextInput v-model="confirmPassword" name="confirmPassword" placeholder="Confirm password" type="password" autocomplete="password" :error="errors.confirmPassword">
              <template #icon-prepend>
                <Icon name="lock" />
              </template>
            </TextInput>
            <div class="flex justify-center">
              <button :disabled="loading" type="submit" class="btn btn-primary btn-wide transition-all">
                Finish setup
                <span v-if="loading" class="loading loading-dots" />
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
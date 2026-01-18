<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useForm} from "vee-validate";
import axios, {type AxiosInstance} from "axios";
import {computed, inject, ref, watch} from "vue";
import {useUserStore} from "@/stores/userStore";
import type { LoginResponse } from "@/types/auth";
import Icon from "@/components/icon.vue";
import {useRoute, useRouter} from "vue-router";
import {useToaster} from "@/stores/toastStore";

enum LoginErrorType {
  NONE,
  INVALID_CREDENTIALS,
  EMAIL_NOT_CONFIRMED,
  EXTERNAL
}

const route = useRoute();
const router = useRouter();
const {createToast} = useToaster();

const schema = toTypedSchema(
  yup.object({
    email: yup
        .string()
        .email("Enter a valid email")
        .required("Email is required"),
    password: yup
        .string()
        .required("Password is required")
        .min(8, "Password must be at least 8 characters")
        .matches(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}/gm, "Password must contain at least one uppercase letter, one lowercase letter, and one number")
  })
);

const { errors, defineField } = useForm({
  validationSchema: schema
});

const [email, emailAttrs] = defineField("email");
const [password, passwordAttrs] = defineField("password");

const routeError = computed<string>(() => {
  return route.query.error?.toString() ?? "";
})

const error = ref({
  msg: routeError.value,
  type: LoginErrorType.NONE
});

watch(routeError, (value: string) => {
  if (value) {
    error.value = {
      msg: value,
      type: LoginErrorType.EMAIL_NOT_CONFIRMED
    };
  }
});

const msg = computed(() => {
  return route.query.msg ?? "";
})

const http = inject<AxiosInstance>("axios", axios.create());
const userStore = useUserStore();

const handleLogin = async () => {
  error.value = {
    msg: "",
    type: LoginErrorType.NONE
  };

  if (errors.value.email || errors.value.password) {
    return;
  }

  try {
    const res = await http.post<LoginResponse>("/auth/login", {
      email: email.value,
      password: password.value
    }, {
      withCredentials: true
    });

    userStore.signIn(res.data);

    router.push({name: 'groups'})
  } catch (e: any) {
    if (e.response.status === 401) {
      error.value = {
        msg: "Invalid email or password",
        type: LoginErrorType.INVALID_CREDENTIALS
      };
    } else if (e.response.status === 403) {
      error.value = {
        msg: "Your account's email is not confirmed",
        type: LoginErrorType.EMAIL_NOT_CONFIRMED
      };
    } else {
      error.value = {
        msg: "An error occurred while logging in",
        type: LoginErrorType.EXTERNAL
      };
    }
  }
}

const handleConfirmationResend = async () => {
  error.value = {
    msg: '',
    type: LoginErrorType.NONE
  }

  try {
    await http.post('/auth/resend-confirmation', { email: email.value });
  } catch (e: any) {
    if (e.response.data.title) {
      createToast(e.response.data.title, e.response.data.message || '', 'error');
    }
  }
}
</script>

<template>
  <form @submit.prevent="handleLogin" class="space-y-4">
    <div v-if="error.msg" class="alert alert-error text-sm shadow-md py-3">
      <div class="flex flex-col items-start gap-1">
        <p class="font-bold">{{error.msg}}</p>
        <button
            v-if="error.type === LoginErrorType.EMAIL_NOT_CONFIRMED"
            @click="handleConfirmationResend"
            class="link link-hover text-xs uppercase font-black"
        >Resend Confirmation Email</button>
      </div>
    </div>

    <div v-if="msg && !error" class="alert alert-success text-sm py-3 shadow-md">
      <span class="font-bold">{{msg}}</span>
    </div>

    <div class="form-control">
      <TextInput v-model="email" name="email" placeholder="Email address" type="email" autocomplete="email" :error="errors.email">
        <template #icon-prepend><Icon name="mail" class="opacity-50" /></template>
      </TextInput>
    </div>

    <div class="form-control">
      <div class="label px-1 py-1 flex justify-between">
        <span class="label-text-alt text-error font-bold">{{errors.password}}</span>
        <RouterLink :to="{name: 'reset-password-request'}" class="label-text-alt link link-primary opacity-70 hover:opacity-100 font-bold">Forgot password?</RouterLink>
      </div>
      <label class="input input-bordered flex items-center gap-2 focus-within:input-primary transition-all" :class="{ 'input-error': !!errors.password }">
        <Icon name="lock" class="opacity-50" />
        <input v-model="password" name="password" placeholder="Password" type="password" autocomplete="password" class="grow" />
      </label>
    </div>

    <button type="submit" class="btn btn-primary btn-block shadow-lg mt-2">
      Sign In
    </button>
  </form>
</template>
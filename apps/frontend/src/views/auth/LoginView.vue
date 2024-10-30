<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useForm} from "vee-validate";
import axios, {type AxiosInstance} from "axios";
import {computed, inject, ref, watch} from "vue";
import {useUserStore} from "@/stores/userStore";
import {type LoginResponse} from "@/types/auth";
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

const routeError = computed(() => {
  return route.query.error?.toString ?? "";
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
    console.error("Failed to log in user");

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
      createToast(e.response.data.title, 'error');
    }
  }
}
</script>

<template>
  <div>
    <div class="text-center">
      <p class="text">New to snowrunner merger? <RouterLink class="btn btn-link btn-sm px-0" :to="{ name: 'register' }">Register here</RouterLink></p>
    </div>
    <form @submit.prevent="handleLogin">
      <div class="flex flex-col gap-4">
        <div class="alert alert-error font-bold" v-if="error.msg">
          <div>
            <p>{{error.msg}}</p>
            <p v-if="error.type === LoginErrorType.EMAIL_NOT_CONFIRMED">Click <a @click="handleConfirmationResend" class="link text-blue-700">here</a> to resend confirmation email to the entered email address</p>
          </div>
        </div>
        <div class="alert alert-success" v-if="msg && !error">
          <span>{{msg}}</span>
        </div>
        <TextInput v-model="email" name="email" placeholder="Email" type="email" autocomplete="email" :error="errors.email">
          <template #icon-prepend>
            <Icon name="mail" />
          </template>
        </TextInput>
        <div>
          <div class="label">
            <span class="label-text text-error">{{errors.password}}</span>
            <span class="ml-auto label-text"><RouterLink :to="{name: 'reset-password-request'}" class="link text-blue-400 transition-colors">Forgot password</RouterLink></span>
          </div>
          <label class="transition-all input input-bordered flex items-center gap-2" :class="{ 'input-error': !!errors.password }">
            <Icon name="lock" />
            <input v-model="password" name="password" placeholder="Password" type="password" autocomplete="password" class="w-full" />
            <slot name="icon-append"></slot>
          </label>
        </div>
        <div class="flex justify-center">
          <button type="submit" class="btn btn-primary btn-wide">Login</button>
        </div>
      </div>
    </form>
  </div>
</template>
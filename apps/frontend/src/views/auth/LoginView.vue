<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useForm} from "vee-validate";
import axios, {type AxiosInstance} from "axios";
import {computed, inject, ref, watch} from "vue";
import {useUserStore} from "@/stores/userStore";
import type {LoginResponse} from "@/types/auth";
import Icon from "@/components/icon.vue";
import {useRoute} from "vue-router";

const route = useRoute();

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
  return route.query.error ?? "";
})

const error = ref(routeError.value);

watch(routeError, (value: string) => {
  if (value) {
    error.value = value;
  }
});

const msg = computed(() => {
  return route.query.msg ?? "";
})

const http = inject<AxiosInstance>("axios", axios.create());
const userStore = useUserStore();

const handleLogin = async () => {
  error.value = "";

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
  } catch (e) {
    console.error("Failed to log in user");

    if (e.response.status === 401) {
      error.value = "Invalid email or password";
    }

    if (e.response.status === 400) {
      error.value = "Email not confirmed";
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
        <div class="alert alert-error" v-if="error">
          <span>{{error}}</span>
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
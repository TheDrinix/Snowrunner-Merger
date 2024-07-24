<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useForm} from "vee-validate";
import axios, {type AxiosInstance} from "axios";
import {inject} from "vue";
import {useUserStore} from "@/stores/userStore";
import type {LoginResponse} from "@/types/auth";

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

const http = inject<AxiosInstance>("axios", axios.create());
const userStore = useUserStore();

const handleLogin = async () => {
  if (errors.value.email || errors.value.password) {
    return;
  }

  console.log("Logging in...");

  const res = await http.post<LoginResponse>("/auth/login", {
    email: email.value,
    password: password.value
  });

  if (res.status < 300) {
    console.log("User logged in successfully");
    console.log(res.data)
    userStore.signIn(res.data);
  } else {
    console.error("Failed to log in user");
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
        <TextInput v-model="email" name="email" placeholder="Email" type="email" autocomplete="email" :error="errors.email" />
        <TextInput v-model="password" name="password" placeholder="Password" type="password" autocomplete="password" :error="errors.password" />
        <div class="flex justify-center">
          <button type="submit" class="btn btn-primary btn-wide">Login</button>
        </div>
      </div>
    </form>
  </div>
</template>
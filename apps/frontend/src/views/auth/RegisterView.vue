<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {useForm} from "vee-validate";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {inject} from "vue";
import axios, {type AxiosInstance} from "axios";

const http = inject<AxiosInstance>("axios", axios.create());

const { errors, defineField } = useForm({
  validationSchema: toTypedSchema(
      yup.object({
        username: yup
            .string()
            .required("Username is required")
            .min(3, "Username must be at least 3 characters"),
        email: yup
            .string()
            .email("Enter a valid email")
            .required("Email is required"),
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
const [email, emailAttrs] = defineField("email");
const [password, passwordAttrs] = defineField("password");
const [confirmPassword, confirmPasswordAttrs] = defineField("confirmPassword");

const handleRegister = async () => {
  if (errors.value.confirmPassword || errors.value.email || errors.value.password || errors.value.username) {
    return;
  }

  console.log("Registering user...");

  const res = await http.post("/auth/register", {
    username: username.value,
    email: email.value,
    password: password.value
  });

  if (res.status < 300) {
    console.log("User registered successfully");
  } else {
    console.error("Failed to register user");
  }
}
</script>

<template>
  <div>
    <div class="text-center">
      <p class="text">Already have an account? <RouterLink class="btn btn-link btn-sm px-0" :to="{ name: 'login' }">Login here</RouterLink></p>
    </div>
    <form @submit.prevent="handleRegister">
      <div class="flex flex-col gap-4">
        <TextInput v-model="username" name="username" placeholder="Username" autocomplete="username" :error="errors.username" />
        <TextInput v-model="email" name="email" placeholder="Email" autocomplete="email" :error="errors.email" />
        <TextInput v-model="password" name="password" placeholder="Password" type="password" autocomplete="password" :error="errors.password" />
        <TextInput v-model="confirmPassword" name="confirmPassword" placeholder="Confirm password" type="password" autocomplete="password" :error="errors.confirmPassword" />
        <div class="flex justify-center">
          <button type="submit" class="btn btn-primary btn-wide">Register</button>
        </div>
      </div>
    </form>
  </div>
</template>

<style scoped>

</style>
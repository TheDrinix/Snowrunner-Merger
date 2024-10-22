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

    await router.push({name: 'login', query: { msg: "Your password has been reset" }});
  } catch (e) {
    err.value = "This link is either invalid or expired. Request a new password reset link."
    startRedirect();
  }

  loading.value = false;
}
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      <h3 class="text-xl">Reset your password</h3>
    </div>
    <div class="card-body pt-4">
      <div v-if="isValid">
        <form @submit.prevent="handlePasswordReset">
          <div class="flex flex-col gap-4">
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
              <button :disabled="!isFormValid || loading" type="submit" class="btn btn-primary btn-wide transition-all">
                Reset
                <span v-if="loading" class="loading loading-dots" />
              </button>
            </div>
          </div>
        </form>
      </div>
      <div v-else class="alert alert-error ">
        <div>
          <h3 class="text-lg font-bold">Link you've entered is invalid or expired. Request a new password reset link.</h3>
          <p class="">You'll be redirected to login page in {{timer}} seconds.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
<script setup lang="ts">
import TextInput from "@/components/forms/TextInput.vue";
import {computed, ref, watch} from "vue";
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
      createToast(e.response.data.title, 'error');
    }
  }

  isLoading.value = false;
}
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      Reset your password
    </div>
    <div class="card-body pt-4">
      <form @submit.prevent="handlePasswordReset">
        <div class="flex flex-col gap-4">
          <TextInput v-model="email" name="email" placeholder="Email" type="email" autocomplete="email">
            <template #icon-prepend>
              <Icon name="mail" />
            </template>
          </TextInput>
          <div class="flex justify-center">
            <button type="submit" class="btn btn-primary btn-wide">Send password reset mail</button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>

</style>
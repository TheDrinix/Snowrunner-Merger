<script setup lang="ts">
import {useUserStore} from "@/stores/userStore";
import {computed, onMounted, ref} from "vue";
import TextInput from "@/components/forms/TextInput.vue";
import UpdateInput from "@/components/forms/UpdateInput.vue";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useForm} from "vee-validate";
import Modal from "@/components/Modal.vue";
import {useLogout} from "@/composables/useLogout";
import {useHttp} from "@/composables/useHttp";
import {useToaster} from "@/stores/toastStore";
import type {User} from "@/types/auth";
import router from "@/router";
import { link } from "fs";

const userStore = useUserStore();
const logout = useLogout();
const http = useHttp();
const { createToast } = useToaster();

const passwordSchema = toTypedSchema(
  yup.object({
    currentPassword: yup
        .string(),
    newPassword: yup
        .string()
        .required("New password is required")
        .min(8, "Password must be at least 8 characters")
        .matches(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}/gm, "Password must contain at least one uppercase letter, one lowercase letter, and one number"),
    confirmPassword: yup
        .string()
        .required("Confirm new password is required")
        .oneOf([yup.ref('newPassword')], "Passwords must match")
  })
);

const usernameSchema = toTypedSchema(
  yup.object({
    username: yup
        .string()
        .required("Username is required")
        .min(3, "Username must be at least 3 characters")
  })
);

const passwordForm = useForm({
  validationSchema: passwordSchema
});

const usernameForm = useForm({
  validationSchema: usernameSchema
});

const [currentPassword, currentPasswordAttrs] = passwordForm.defineField("currentPassword");
const [newPassword, newPasswordAttrs] = passwordForm.defineField("newPassword");
const [confirmPassword, confirmPasswordAttrs] = passwordForm.defineField("confirmPassword");

const [username, usernameAttrs] = usernameForm.defineField("username");

const isAccountDeleteModalOpen = ref(false);
const OAuthUnlinkModalState = ref<string | null>(null);
const isOAuthUnlinkModalOpen = computed({
  get: () => OAuthUnlinkModalState.value !== null,
  set: (val: boolean) => {
    if (!val) {
      OAuthUnlinkModalState.value = null;
    }
  }
});

const user = computed(() => userStore.user);

const updating = ref(0);
const isLoading = ref(false);

const isPasswordFormValid = computed(() => {
  return passwordForm.isFieldValid('currentPassword')
      && passwordForm.isFieldValid('newPassword')
      && passwordForm.isFieldValid('confirmPassword');
})

const handleUsernameUpdate = async () => {
  const validation = await usernameForm.validate();

  if (!validation.valid) return;

  isLoading.value = true;

  try {
    const res = await http.patch<User>('/user/username', { username: username.value });
    console.log(res);

    userStore.storeUser(res.data);

    createToast('Username updated successfully', 'success');
  } catch (e: any) {
    createToast(e.response.data.title, 'error');
  } finally {
    isLoading.value = false;
    updating.value = 0;
  }
}

const handlePasswordUpdate = async () => {
  const validation = await passwordForm.validate();

  if (!validation.valid) return;

  isLoading.value = true;

  try {
    await http.patch<User>('/user/password', {
      currentPassword: currentPassword.value,
      newPassword: newPassword.value
    });

    createToast('Password updated successfully', 'success');
  } catch (e: any) {
    createToast(e.response?.data.title, '', 'error', 10000);
  } finally {
    isLoading.value = false;
    updating.value = 0;
  }
}

const handleAccountDelete = async () => {
  isLoading.value = true;

  try {
    await http.delete('/user');
    userStore.logout();

    createToast('Account deleted successfully', 'success');
    router.push({ name: 'home' });
  } catch (e: any) {
    createToast(e.response.data.title, 'error');
  } finally {
    isLoading.value = false;
    isAccountDeleteModalOpen.value = false;
  }
}

const oauthLoading = ref(false);
const oauthProviders = computed(() => userStore.oauthProviders);
const linkedProviders = ref({} as Record<string, boolean>);

const handleLinkOAuthAccount = async (provider: string) => {
  try {
    const redirectUrl = new URL(`/auth/${provider}/link`, window.location.origin);

    const res = await http.get<string>(`/auth/${provider}/signin/`, {
      withCredentials: true,
      params: {
        callbackUrl: redirectUrl.toString()
      }
    });

    // Redirect the user to the OAuth provider's sign-in page
    window.location.href = res.data;
  } catch (e) {
    createToast('Linking failed', `There was an error trying to link your ${provider} account, please try again later.`, 'error', 10);
  }
}

const handleOAuthUnlink = async () => {
  const provider = OAuthUnlinkModalState.value;

  if (!provider) return;

  try {
    await http.post(`/auth/${provider}/unlink`, {});

    linkedProviders.value[provider] = false;
    createToast('Unlink success', `${provider.charAt(0).toUpperCase() + provider.slice(1)} account unlinked successfully`, 'success');
    isOAuthUnlinkModalOpen.value = false;
  } catch (e) {
    createToast('Unlink failed', `There was an error trying to unlink your ${provider} account, please try again later.`, 'error', 10);
  }
  
  OAuthUnlinkModalState.value = provider;
}

onMounted(async () => {
  oauthLoading.value = true;

  if (!oauthProviders.value) {
    await userStore.fetchOAuthProviders();

    for (const provider of oauthProviders.value || []) {
      linkedProviders.value[provider] = false;
    }
  }

  const res = await http.get<string[]>('/user/oauth/providers');

  for (const provider of res.data) {
    linkedProviders.value[provider] = true;
  }

  oauthLoading.value = false;
})
</script>

<template>
  <div class="max-w-3xl mx-auto py-10 px-4">
    <div class="card bg-base-100 shadow-xl border border-base-300 mb-8 overflow-hidden">
      <div class="h-24 bg-gradient-to-r from-primary/30 to-secondary/30"></div>
      <div class="px-8 pb-8 flex flex-col md:flex-row items-end gap-6 -mt-12">
        <div class="avatar placeholder">
          <div class="bg-neutral text-neutral-content rounded-2xl w-32 h-32 shadow-2xl ring-8 ring-base-100">
            <span class="text-5xl font-black">{{ user?.username?.charAt(0).toUpperCase() }}</span>
          </div>
        </div>
        <div class="flex-grow mb-2">
          <h2 class="text-3xl font-black">{{ user?.username }}</h2>
          <p class="opacity-60 flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>
            {{ user?.email }}
          </p>
        </div>
        <button @click="logout" class="btn btn-ghost btn-sm mb-2">Logout</button>
      </div>
    </div>

    <div class="grid grid-cols-1 gap-8">

      <section class="card bg-base-200 border border-base-300 shadow-sm">
        <div class="card-body p-6">
          <h3 class="flex items-center gap-2 font-bold text-lg mb-4 text-primary uppercase tracking-wider">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
            Basic Information
          </h3>

          <div class="space-y-6">
            <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 py-4 border-b border-base-300 last:border-0">
              <div>
                <p class="text-sm font-bold opacity-50 uppercase">Username</p>
                <p v-if="updating !== 1" class="text-lg">{{ user?.username }}</p>
              </div>

              <div v-if="updating === 1" class="w-full md:w-2/3">
                <UpdateInput v-model="username" size="sm" name="username" placeholder="Username" @update="handleUsernameUpdate" @cancel="updating = 0" :error="usernameForm.errors.value.username" />
              </div>
              <button v-else @click="updating = 1" class="btn btn-sm btn-outline">Change</button>
            </div>

            <div class="flex flex-col md:flex-row md:items-start justify-between gap-4 py-4">
              <div class="flex-grow">
                <p class="text-sm font-bold opacity-50 uppercase">Password</p>
                <p v-if="updating !== 2" class="text-lg">••••••••••••</p>

                <div v-if="updating === 2" class="mt-4 bg-base-300 p-4 rounded-xl border border-base-100 shadow-inner">
                  <form @submit.prevent="handlePasswordUpdate" class="space-y-4">
                    <TextInput type="password" autocomplete="current-password" v-model="currentPassword" name="current-password" placeholder="Current password" :error="passwordForm.errors.value.currentPassword" />
                    <TextInput type="password" autocomplete="new-password" v-model="newPassword" name="new-password" placeholder="New password" :error="passwordForm.errors.value.newPassword" />
                    <TextInput type="password" autocomplete="new-password" v-model="confirmPassword" name="confirm-password" placeholder="Confirm new password" :error="passwordForm.errors.value.confirmPassword" />
                    <div class="flex justify-end gap-2">
                      <button @click="updating = 0" type="button" class="btn btn-ghost btn-sm">Cancel</button>
                      <button class="btn btn-primary btn-sm" :disabled="!isPasswordFormValid">Update Password</button>
                    </div>
                  </form>
                </div>
              </div>
              <button v-if="updating !== 2" @click="updating = 2" class="btn btn-sm btn-outline">Change</button>
            </div>
          </div>
        </div>
      </section>

      <section class="card bg-base-200 border border-base-300 shadow-sm">
        <div class="card-body p-6">
          <h3 class="flex items-center gap-2 font-bold text-lg mb-4 text-primary uppercase tracking-wider">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1" /></svg>
            Linked Accounts
          </h3>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-2">
            <div v-for="provider in oauthProviders" :key="provider"
                 class="p-4 bg-base-100 rounded-xl border border-base-300 flex items-center justify-between">
              <div>
                <p class="font-bold capitalize">{{ provider }}</p>
                <span class="text-[10px] uppercase opacity-40 font-black">Authentication</span>
              </div>

              <button v-if="linkedProviders[provider]"
                      class="btn btn-error btn-xs btn-outline"
                      @click="OAuthUnlinkModalState = provider">Unlink</button>
              <button v-else
                      class="btn btn-primary btn-xs"
                      @click="handleLinkOAuthAccount(provider)">Link</button>
            </div>
          </div>
        </div>
      </section>

      <section class="card border border-error/30 bg-error/5 shadow-sm">
        <div class="card-body p-6">
          <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
            <div>
              <h3 class="text-error font-bold text-lg uppercase tracking-tight">Danger Zone</h3>
              <p class="text-sm opacity-70">Permanently delete your account and all shared saves.</p>
            </div>
            <button class="btn btn-error btn-sm px-8" @click="isAccountDeleteModalOpen = true">Delete Account</button>
          </div>
        </div>
      </section>

    </div>

    <Modal v-model="isAccountDeleteModalOpen">
      <div class="p-6 text-center">
        <div class="w-16 h-16 bg-error/10 text-error rounded-full flex items-center justify-center mx-auto mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
        </div>
        <h3 class="text-2xl font-black mb-2">Wait! Are you sure?</h3>
        <p class="opacity-60 mb-6">Deleting your account is irreversible. All your groups and saves will be permanently removed.</p>
        <div class="flex flex-col gap-2">
          <button @click="handleAccountDelete" class="btn btn-error btn-block">Yes, Delete Everything</button>
          <button @click="isAccountDeleteModalOpen = false" class="btn btn-ghost btn-block">I've changed my mind</button>
        </div>
      </div>
    </Modal>

    <Modal v-model="isOAuthUnlinkModalOpen">
      <div class="p-6">
        <h3 class="text-xl font-bold mb-4">Unlink {{ OAuthUnlinkModalState }}?</h3>
        <p class="mb-6 opacity-70">You will no longer be able to log in using your {{ OAuthUnlinkModalState }} account.</p>
        <div class="flex justify-end gap-3">
          <button @click="OAuthUnlinkModalState = null" class="btn btn-ghost">Cancel</button>
          <button @click="handleOAuthUnlink" class="btn btn-error px-8">Confirm Unlink</button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<style scoped>

</style>
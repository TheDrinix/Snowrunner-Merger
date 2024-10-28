<script setup lang="ts">
import {useUserStore} from "@/stores/userStore";
import {computed, ref} from "vue";
import TextInput from "@/components/forms/TextInput.vue";
import UpdateInput from "@/components/forms/UpdateInput.vue";
import {toTypedSchema} from "@vee-validate/yup";
import * as yup from "yup";
import {useForm} from "vee-validate";
import Modal from "@/components/Modal.vue";
import {useLogout} from "@/composables/useLogout";

const userStore = useUserStore();
const logout = useLogout();

const passwordSchema = toTypedSchema(
  yup.object({
    currentPassword: yup
        .string()
        .required("Current password is required"),
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

const passwordForm = useForm({
  validationSchema: passwordSchema
});

const [currentPassword, currentPasswordAttrs] = passwordForm.defineField("currentPassword");
const [newPassword, newPasswordAttrs] = passwordForm.defineField("newPassword");
const [confirmPassword, confirmPasswordAttrs] = passwordForm.defineField("confirmPassword");

const isAccountDeleteModalOpen = ref(false);

const user = computed(() => userStore.user);

const updating = ref(0);

const isPasswordFormValid = computed(() => {
  return passwordForm.isFieldValid('currentPassword')
      && passwordForm.isFieldValid('newPassword')
      && passwordForm.isFieldValid('confirmPassword');
})

const handleUsernameUpdate = async () => {

}

const handlePasswordUpdate = async () => {
  const validation = await passwordForm.validate();

  if (!validation.valid) return;


}

const handleAccountDelete = async () => {

}
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      <span class="card-title">Your account</span>
    </div>
    <div class="card-body pt-4">
      <div>
        <h3>Account details</h3>
        <div class="mt-2 flex flex-col gap-4 w-full">
          <p>Email: {{ user?.email }}</p>
          <div class="w-full flex justify-between" v-if="updating != 1">
            <span>Username: {{ user?.username }}</span>
            <button @click="() => updating = 1" class="btn btn-primary btn-sm">Change</button>
          </div>
          <div v-else>
            <UpdateInput size="sm" name="username" placeholder="Username" @update="handleUsernameUpdate" @cancel="() => updating = 0" />
          </div>
          <div class="w-full flex justify-between" v-if="updating != 2">
            <span>Password: ********</span>
            <button class="btn btn-primary btn-sm" @click="() => updating = 2">Change</button>
          </div>
          <div v-else>
            <form @submit.prevent>
              <TextInput v-model="currentPassword" name="current-password" placeholder="Current password" :error="passwordForm.errors.value.currentPassword" />
              <TextInput v-model="newPassword" name="new-password" placeholder="New password" :error="passwordForm.errors.value.newPassword" />
              <TextInput v-model="confirmPassword" name="confirm-password" placeholder="Confirm new password" :error="passwordForm.errors.value.confirmPassword" />
              <div class="flex justify-end gap-4 mt-4">
                <button class="btn btn-primary btn-sm" :disabled="!isPasswordFormValid">Update</button>
                <button @click="() => updating = 0" class="btn btn-secondary btn-sm">Cancel</button>
              </div>
            </form>
          </div>
          <hr class="divider border-none m-0" />
          <div class="flex gap-4 justify-end">
            <button class="btn btn-error btn-sm" @click="() => isAccountDeleteModalOpen = true">Delete account</button>
            <button class="btn btn-primary btn-sm" @click="logout">Logout</button>
          </div>
          <Modal v-model="isAccountDeleteModalOpen">
            <div class="p-4">
              <h3 class="text-lg font-bold">Are you sure you want to delete your account?</h3>
              <div class="flex justify-end gap-4 mt-4">
                <button class="btn btn-error btn-sm">Delete</button>
                <button @click="() => isAccountDeleteModalOpen = false" class="btn btn-secondary btn-sm">Cancel</button>
              </div>
            </div>
          </Modal>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
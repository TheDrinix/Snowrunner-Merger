<script setup lang="ts">
import {useToaster} from "@/stores/toastStore";
import Toast from "@/components/Toast.vue";
import {computed} from "vue";

const toaster = useToaster();

const toasts = computed(() => toaster.toasts);
</script>

<template>
  <div class="fixed top-4 left-0 md:top-auto md:left-auto md:bottom-4 md:right-4 px-[5vw] md:px-0 z-50">
    <div class="flex flex-col md:flex-col-reverse items-end">
      <TransitionGroup enter-active-class="transform ease-out duration-300 transition"
                       enter-from-class="opacity-0 translate-x-2"
                       enter-to-class="opacity-100 translate-x-0"
                       leave-active-class="transition ease-in duration-100"
                       leave-from-class="opacity-100"
                       leave-to-class="opacity-0">
        <Toast v-for="toast in toasts" :key="toast.id" v-bind="toast" @close="toaster.removeToast" />
      </TransitionGroup>
    </div>
  </div>
</template>

<style scoped>

</style>
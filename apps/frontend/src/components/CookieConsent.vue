<script setup lang="ts">
import {onMounted, ref} from "vue";

const hidden = ref(false);
const playAnimation = ref(true);

onMounted(() => {
  if (localStorage.getItem("cookie-consent") === "true") {
    hidden.value = true;
    playAnimation.value = false;
  }
});

const handleAccept = () => {
  localStorage.setItem("cookie-consent", "true");
  hidden.value = true;
};
</script>

<template>
  <Transition v-if="playAnimation" name="slide-fade">
    <div v-if="!hidden" class="fixed flex items-center rounded bottom-4 left-2 bg-base-300 p-4 text-center">
      <div>
        <p class="text-sm">We use essential cookies to provide and secure our service.</p>
        <p class="text-sm">By using our site, you consent to the use of these cookies.</p>
      </div>
      <div class="flex items-center justify-end ml-4">
        <button @click="handleAccept" class="btn btn-sm btn-outline btn-primary">Accept</button>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
  .slide-fade-enter-active {
    transition: all 0.3s ease-out;
  }

  .slide-fade-leave-active {
    transition: all 0.3s ease-out;
  }

  .slide-fade-enter-from,
  .slide-fade-leave-to {
    opacity: 0;
    transform: translateX(-20px);
  }
</style>
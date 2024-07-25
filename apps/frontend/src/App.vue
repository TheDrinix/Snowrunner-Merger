<script setup lang="ts">
import { RouterView } from 'vue-router'
import Header from "@/components/Header.vue";
import Footer from "@/components/Footer.vue";
import {onBeforeMount} from "vue";
import {useUserStore} from "@/stores/userStore";

const userStore = useUserStore();

onBeforeMount(async () => {
  await userStore.refreshToken();
});
</script>

<template>
  <Header />
  <div class="container mx-auto mt-4">
    <main class="pb-3">
      <RouterView />
    </main>
  </div>
  <Footer />
</template>

<style scoped>
</style>

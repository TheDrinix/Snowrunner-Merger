<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Header from "@/components/Header.vue";
import Footer from "@/components/Footer.vue";
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

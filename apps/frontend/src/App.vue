<script setup lang="ts">
import { RouterView } from 'vue-router'
import Header from "@/components/Header.vue";
import Footer from "@/components/Footer.vue";
import Toaster from "@/components/Toaster.vue";

</script>

<template>
  <div class="relative min-h-screen pb-16">
    <Header />
    <div class="container mx-auto mt-4">
      <main class="pb-3">
        <RouterView />
      </main>
    </div>
    <Footer />
    <Toaster />
  </div>
</template>

<style scoped>
</style>

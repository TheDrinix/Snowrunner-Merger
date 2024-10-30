<script setup lang="ts">
import { RouterView } from 'vue-router'
import Header from "@/components/Header.vue";
import Footer from "@/components/Footer.vue";
import Toaster from "@/components/Toaster.vue";
import CookieConsent from "@/components/CookieConsent.vue";

</script>

<template>
  <div class="relative min-h-screen pb-32">
    <Header />
    <div class="container mx-auto mt-4">
      <main class="pb-3">
        <RouterView />
      </main>
    </div>
    <Footer />
    <Toaster />
    <CookieConsent />
  </div>
</template>

<style scoped>
</style>

<script setup lang="ts">
const year = new Date().getFullYear();
</script>

<template>
  <footer class="footer flex p-4 items-center bg-base-200 mt-auto">
    <div>
      <p class="text font-medium">&copy; {{ year }} Snowrunner Merger</p>
    </div>
    <nav class="ml-auto">
      <h6 class="footer-title mb-0">Github links</h6>
      <a href="https://github.com/TheDrinix/snowrunner-merger-api" class="link link-hover">API Source code</a>
      <a href="https://github.com/TheDrinix/snowrunner-merger-web" class="link link-hover">Website source code</a>
    </nav>
    <nav>
      <h6 class="footer-title mb-0">Legal mumbo jumbo</h6>
      <RouterLink class="link link-hover" to="/privacy">Privacy policy</RouterLink>
      <RouterLink class="link link-hover" to="/terms-of-use">Terms of service</RouterLink>
    </nav>
  </footer>
</template>

<style scoped>

</style>
<script setup lang="ts">
const year = new Date().getFullYear();
</script>

<template>
  <footer class="footer p-4 bg-base-200 absolute bottom-0">
    <aside class="items-center grid-flow-col">
      <p class="h-6">&copy; {{year}} - SnowrunnerSaveMerger<!-- - <a class="inline underline" asp-area="" asp-page="/Privacy">Privacy</a>--></p>
    </aside>
  </footer>
</template>

<style scoped>

</style>
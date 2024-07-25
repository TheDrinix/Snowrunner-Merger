<script setup lang="ts">
const props = defineProps<{
  name: string;
}>();
</script>

<template>
  <span class="material-symbols-outlined">
    {{ name }}
  </span>
</template>

<style scoped>

</style>
<script setup lang="ts">
const props = defineProps<{
  isOwner?: boolean
}>();
</script>

<template>
  <div class="card w-full bg-base-100">
    <div class="card-body p-4 flex-row justify-between">
      <div class="skeleton h-5 m-1 w-36" />
      <div class="card-actions">
        <div v-if="isOwner" class="skeleton h-5 my-1 w-12" />
        <div class="skeleton h-5 my-1 w-12" />
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
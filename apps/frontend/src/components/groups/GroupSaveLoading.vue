<script setup lang="ts">

</script>

<template>
  <div class="card w-full bg-base-100 shadow-xl">
    <div class="card-body pb-6 flex-row justify-between">
      <div class="flex flex-col flex-wrap w-4/5 gap-2">
        <div class="skeleton h-6 w-20"></div>
        <div class="skeleton h-4 w-72"></div>
        <div class="skeleton h-4 w-24"></div>
      </div>
      <div class="flex flex-col justify-around items-center">
        <button disabled class="btn btn-outline btn-secondary">Merge</button>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
<script setup lang="ts">

</script>

<template>
  <header>
    <nav class="navbar bg-base-200">
      <div class="container mx-auto">
        <div class="flex-1">
          <RouterLink :to="{ name: 'home'}" />
        </div>
        <div class="flex-none">
          Sign-in bar
        </div>
      </div>
    </nav>
  </header>
</template>

<style scoped>

</style>
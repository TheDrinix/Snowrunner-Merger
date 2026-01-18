<script setup lang="ts">

</script>

<template>
  <div class="max-w-md mx-auto py-20 px-4">
    <div class="card bg-base-200 shadow-2xl border border-base-300 text-center overflow-visible">
      <div class="flex justify-center -mt-16">
        <div class="bg-primary text-primary-content p-6 rounded-3xl shadow-2xl ring-8 ring-base-100 rotate-3">
          <svg class="w-16 h-16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
          </svg>
        </div>
      </div>

      <div class="card-body items-center pt-8">
        <h2 class="text-3xl font-black uppercase tracking-tighter">Check your mail!</h2>
        <p class="opacity-70 text-balance">
          We've sent a confirmation link to your email address. Please verify your account to start merging saves.
        </p>

        <div class="divider w-full opacity-20"></div>

        <div class="card-actions w-full">
          <RouterLink :to="{ name: 'login' }" class="btn btn-outline btn-block">
            Return to Login
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
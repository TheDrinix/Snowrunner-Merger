<script setup lang="ts">
</script>

<template>
  <div>
    <section class="hero min-h-[60vh] bg-base-300 relative overflow-hidden rounded-md">
      <div class="hero-overlay bg-gradient-to-b from-primary/20 to-base-300"></div>

      <div class="hero-content text-center">
        <div class="max-w-2xl">
          <h1 class="text-5xl md:text-6xl font-black tracking-tighter leading-none mb-6">
            Merge Your Saves <br/>
            <span class="text-primary italic">Seamlessly.</span>
          </h1>
          <p class="text-lg opacity-80 mb-8 leading-relaxed">
            Snowrunner Merger lets you combine game progress with friends.
            Keep your adventure moving, host or not, exactly where you left off.
          </p>
          <div class="flex justify-center gap-4">
            <RouterLink :to="{ name: 'groups' }" class="btn btn-primary btn-lg shadow-xl px-8">
              Get Started
            </RouterLink>
            <a href="#features" class="btn btn-ghost btn-lg">
              How it works
            </a>
          </div>
        </div>
      </div>
    </section>

    <main id="features" class="container mx-auto px-6 py-20">
      <div class="text-center mb-16">
        <h3 class="text-xs font-black uppercase tracking-[0.3em] text-primary mb-2">Capabilities</h3>
        <h2 class="text-2xl font-bold">Engineered for those who like to Mud Up</h2>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="card bg-base-200 border border-base-300 hover:border-secondary/50 transition-all group">
          <div class="card-body items-center text-center">
            <div class="w-16 h-16 rounded-2xl bg-secondary/10 text-secondary flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
              </svg>
            </div>
            <h4 class="card-title">Easy Merging</h4>
            <p class="text-sm opacity-70">Combine progress from multiple saves effortlessly. Bring your achievements together in seconds.</p>
          </div>
        </div>

        <div class="card bg-base-200 border border-base-300 hover:border-secondary/50 transition-all group">
          <div class="card-body items-center text-center">
            <div class="w-16 h-16 rounded-2xl bg-secondary/10 text-secondary flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" /></svg>
            </div>
            <h4 class="card-title">Simple to Use</h4>
            <p class="text-sm opacity-70">Point to your save folder and let us handle the rest. No manual JSON editing required.</p>
          </div>
        </div>

        <div class="card bg-base-200 border border-base-300 hover:border-secondary/50 transition-all group">
          <div class="card-body items-center text-center">
            <div class="w-16 h-16 rounded-2xl bg-secondary/10 text-secondary flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
            </div>
            <h4 class="card-title">Save History</h4>
            <p class="text-sm opacity-70">Store up to 3 saves per group. Keep backups of your progression milestones safely in the cloud.</p>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();

const groupId = computed(() => {
  return route.params.id as string;
});
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:w-full bg-base-100 shadow-xl border border-success/30 overflow-hidden">
    <div class="bg-success/10 p-8 text-center border-b border-success/20">
      <div class="inline-flex items-center justify-center w-16 h-16 bg-success text-success-content rounded-full mb-4 shadow-lg">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
        </svg>
      </div>
      <h2 class="text-3xl font-black">Merge Complete!</h2>
      <p class="text-base-content/70 mt-2">Your new Snowrunner save is ready for action.</p>
    </div>

    <div class="card-body">
      <div class="flex flex-col lg:flex-row gap-8">

        <div class="w-full lg:w-3/5">
          <h3 class="font-bold text-xl mb-4">Final Instructions</h3>

          <div class="alert alert-warning shadow-sm mb-6 bg-warning/10 border-warning/20">
            <svg xmlns="http://www.w3.org/2000/svg" class="stroke-warning shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
            <span class="text-sm text-base-content"><strong>Pro Tip:</strong> Back up your original save folder before overwriting anything!</span>
          </div>

          <ul class="steps steps-vertical space-y-2">
            <li class="step step-primary">
              <div class="text-left ml-4">
                <p class="font-bold">Extract the Download</p>
                <p class="text-sm opacity-70">Open the ZIP file you just downloaded and locate the save files inside.</p>
              </div>
            </li>
            <li class="step step-primary">
              <div class="text-left ml-4">
                <p class="font-bold">Open Save Directory</p>
                <p class="text-sm opacity-70">Navigate back to your Snowrunner save folder (use the paths on the right).</p>
              </div>
            </li>
            <li class="step step-primary">
              <div class="text-left ml-4">
                <p class="font-bold">Paste & Overwrite</p>
                <p class="text-sm opacity-70">Drag the files from the ZIP into your save folder. Choose "Replace" when prompted by Windows.</p>
              </div>
            </li>
            <li class="step">
              <div class="text-left ml-4">
                <p class="font-bold">Start Snowrunner</p>
                <p class="text-sm opacity-70">Launch the game and verify your progress in the selected slot.</p>
              </div>
            </li>
          </ul>
        </div>

        <div class="divider lg:divider-horizontal mx-0"></div>

        <div class="w-full lg:w-2/5">
          <div class="bg-base-200 p-6 rounded-2xl border border-base-300">
            <h3 class="text-xs font-black uppercase tracking-widest opacity-60 mb-4">Folder Path Reminder</h3>

            <div role="tablist" class="tabs tabs-lifted tabs-sm">
              <input type="radio" name="success_path_tabs" role="tab" class="tab" aria-label="Steam" checked />
              <div role="tabpanel" class="tab-content bg-base-100 border-base-300 rounded-box p-4">
                <code class="text-xs break-all block mb-2 font-mono">Steam/userdata/&lt;steam_id&gt;/1465360/remote</code>
              </div>

              <input type="radio" name="success_path_tabs" role="tab" class="tab" aria-label="Epic/Other" />
              <div role="tabpanel" class="tab-content bg-base-100 border-base-300 rounded-box p-4">
                <code class="text-xs break-all block mb-2 font-mono">Documents/My games/Snowrunner/base/storage</code>
              </div>
            </div>

            <div class="mt-6 flex flex-col gap-2">
              <RouterLink :to="{ name: 'group', params: { id: groupId } }" class="btn btn-outline btn-sm">Return to Group Page</RouterLink>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
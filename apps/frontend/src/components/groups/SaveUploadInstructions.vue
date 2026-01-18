<script setup lang="ts">

</script>

<template>
  <div class="w-full lg:w-2/5">
    <div class="bg-base-100 p-6 rounded-2xl border border-base-300 h-full">
      <h3 class="text-sm font-black uppercase tracking-widest text-primary mb-4">Quick Guide</h3>

      <ul class="steps steps-vertical space-y-4">
        <li class="step step-primary">
          <div class="text-left ml-4">
            <p class="font-bold text-sm">Find your folder</p>
            <div role="tablist" class="tabs tabs-lifted tabs-sm mt-2">
              <input type="radio" name="path_tabs" role="tab" class="tab" aria-label="Steam" checked />
              <div role="tabpanel" class="tab-content bg-base-100 border-base-300 rounded-box p-3">
                <code class="text-xs break-all opacity-80">Steam/userdata/&lt;steam_id&gt;/1465360/remote</code>
              </div>

              <input type="radio" name="path_tabs" role="tab" class="tab" aria-label="Epic/Other" />
              <div role="tabpanel" class="tab-content bg-base-100 border-base-300 rounded-box p-3">
                <code class="text-xs break-all opacity-80">Documents/My games/Snowrunner/base/storage</code>
              </div>
            </div>
            <p class="text-sm mt-2 italic">Select the folder with long alphanumeric name.</p>
          </div>
        </li>
        <li class="step step-primary">
          <div class="text-left ml-4">
            <p class="font-bold text-sm">Select source save slot</p>
            <p class="text-xs opacity-70">The save game you want to upload to the group.</p>
          </div>
        </li>
        <li class="step step-primary">
          <div class="text-left ml-4">
            <p class="font-bold text-sm">Write up a description</p>
            <p class="text-xs opacity-70">Describe the progression of the save (or whatever else you want).</p>
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>

<style scoped>

</style>
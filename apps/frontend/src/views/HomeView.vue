<script setup lang="ts">
</script>

<template>
  <div class="max-w-screen-lg mx-auto flex flex-col gap-4 px-4">
    <div>
      <h1 class="text-2xl font-bold ">Welcome to Snowrunner merger</h1>
      <p>Snowrunner Merger makes it easy to share your game progress with friends. Upload your save files to a shared group and let the Merger handle the rest.</p>
    </div>
    <div>
      <h2 class="text-lg font-bold">What is snowrunner merger?</h2>
      <p>Snowrunner merger is a tool for sharing Snowrunner save progression between players by taking progression data from host's save file and merging it into saves of other players.</p>
      <p><span class="font-medium">But why you may ask.</span> When playing coop Snowrunner, the progression is not shared between individual players playing, only host gets the progression, that's why I created this tool for sharing this progression.</p>
    </div>
    <div>
      <h2 class="text-lg font-bold">How it works?</h2>
      <ol class="list-decimal ml-6">
        <li class="list-item">
          You and your friends create an account
        </li>
        <li>Hosting player creates a group and uploads a save file there</li>
        <li>Others then join the group and can merge their save files with the ones stored in the group</li>
        <li>After merging, the save file is downloaded and can be used in Snowrunner</li>
      </ol>
    </div>
    <div class="flex flex-col items-center">
      <h2 class="text-lg font-bold">Get started now!</h2>
      <p class="text-center">Click the button below to create an account and start sharing your progression with friends.</p>
      <RouterLink to="/register" class="btn btn-primary btn-outline mt-2">Create account</RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import type {StoredSave} from "@/types/saves";
import {useTimeAgo} from "@vueuse/core";

const props = defineProps<{
  save: StoredSave,
  idx: number,
  groupId: string
}>();

const timeAgo = useTimeAgo(props.save.uploadedAt);
</script>

<template>
  <div class="card w-full bg-base-100 shadow-xl">
    <div class="card-body pb-6 flex-row justify-between">
      <div class="flex flex-col flex-wrap w-4/5">
        <h3 class="text-lg font-medium">Save {{idx + 1}}</h3>
        <p class="break-words">{{save.description}}</p>
        <p class="text-sm text-neutral-content mt-2">Uploaded {{timeAgo}}</p>
      </div>
      <div class="flex flex-col justify-around items-center gap-4">
        <slot name="actions" />
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
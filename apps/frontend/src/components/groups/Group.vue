<script setup lang="ts">
import type { Group } from "@/types/groups";
import { ref } from "vue";

const props = defineProps<{
  group: Group,
  isOwner?: boolean
}>();

const tooltipText = ref("Click to copy")

const handleCopy = () => {
  tooltipText.value = "Copied!";
  
  console.log(tooltipText.value);
  setTimeout(() => {
    tooltipText.value = "Click to copy"
  }, 2000);
  
  navigator.clipboard.writeText(props.group.inviteCode);
}
</script>

<template>
  <div class="group flex items-center justify-between p-4 bg-base-100 border border-base-200 rounded-xl hover:border-primary/50 hover:shadow-md transition-all duration-200">
    <div class="flex flex-col gap-1">
      <h3 class="font-bold text-lg group-hover:text-primary transition-colors">{{ group.name }}</h3>

      <div v-if="isOwner" class="flex items-center gap-2">
        <span class="text-[10px] uppercase tracking-wider font-black opacity-40">Invite Code:</span>
        <div class="tooltip tooltip-right" :data-tip="tooltipText">
          <code @click.prevent="handleCopy"
                class="bg-base-200 px-2 py-0.5 rounded text-xs font-mono cursor-pointer hover:bg-primary hover:text-primary-content transition-colors">
            {{ group.inviteCode }}
          </code>
        </div>
      </div>
    </div>

    <div class="flex items-center">
      <RouterLink
          :to="{ name: 'group', params: { id: group.id } }"
          :class="isOwner ? 'btn-primary' : 'btn-ghost border-base-300'"
          class="btn btn-sm shadow-sm"
      >
        {{ isOwner ? "Manage" : "Enter" }}
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
        </svg>
      </RouterLink>
    </div>
  </div>
</template>

<style scoped>

</style>
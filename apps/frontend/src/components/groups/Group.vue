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
  
  navigator.clipboard.writeText(props.group.id);
}
</script>

<template>
  <div class="card w-full bg-base-100">
    <div class="card-body p-4 flex-row justify-between">
      <div>
        <h3 class="card-title">{{ group.name }}</h3>
        <div class="text-sm text-neutral-content" v-if="isOwner">
          <span>Invite code: </span>
          <div class="tooltip tooltip-bottom" :data-tip="tooltipText">
            <a class="cursor-pointer hover:underline" @click.prevent="handleCopy">{{group.id}}</a>
          </div>
        </div>
      </div>
      <div class="card-actions items-center">
        <div class="join join-vertical md:join-horizontal">
          <RouterLink :to="{ name: 'group', params: { id: group.id } }" class="btn btn-sm join-item">{{ isOwner ? "Manage" : "View" }}</RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
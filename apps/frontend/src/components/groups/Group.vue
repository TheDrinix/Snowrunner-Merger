<script setup lang="ts">
import { type Group } from "@/types/groups";

const props = defineProps<{
  group: Group,
  isOwner?: boolean
}>();
</script>

<template>
  <div class="card w-full bg-base-100">
    <div class="card-body p-4 flex-row justify-between">
      <h3 class="card-title">{{ group.name }}</h3>
      <div class="card-actions">
        <RouterLink :to="{ name: 'group', params: { id: group.id } }" class="btn btn-sm">View</RouterLink>
        <RouterLink v-if="isOwner" :to="{ name: 'group-manage', params: { id: group.id } }" class="btn btn-sm">Manage</RouterLink>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
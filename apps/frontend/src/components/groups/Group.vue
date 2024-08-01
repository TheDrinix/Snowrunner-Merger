<script setup lang="ts">
import { type Group } from "@/types/groups";

const props = defineProps<{
  group: Group,
  isOwner?: boolean
}>();

const handleCopy = () => {
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
          <div class="tooltip tooltip-bottom" data-tip="Click to copy">
            <a class="cursor-pointer hover:underline" @click.prevent="handleCopy">{{group.id}}</a>
          </div>
        </div>
      </div>
      <div class="card-actions items-center">
        <RouterLink :to="{ name: 'group', params: { id: group.id } }" class="btn btn-sm">View</RouterLink>
        <RouterLink v-if="isOwner" :to="{ name: 'group-manage', params: { id: group.id } }" class="btn btn-sm">Manage</RouterLink>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
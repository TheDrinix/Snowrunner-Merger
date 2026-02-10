<script setup lang="ts">
import type { NavigationLink } from '@/types/navigation';

const props = defineProps<{
  links: NavigationLink[];
}>();
</script>

<template>
  <div class="text-sm breadcrumbs mb-1 opacity-60">
    <ul>
      <li v-for="(link, index) in props.links" :key="index">
        <RouterLink v-if="link.to" :to="link.to">{{ link.name }}</RouterLink>
        <span v-else>{{ link.name }}</span>
      </li>
    </ul>
  </div>
</template>
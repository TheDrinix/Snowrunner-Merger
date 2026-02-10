<script setup lang="ts">
import Breadcrumb from './Breadcrumb.vue';
import type { NavigationLink } from '@/types/navigation';

const props = defineProps<{
  title: string;
  loading: boolean;
  links?: NavigationLink[];
}>();
</script>

<template>
  <div>
    <Breadcrumb v-if="links?.length" :links />
    <h2 class="text-3xl font-black flex items-center gap-3">
      <div class="w-3 h-8 bg-secondary rounded-full"></div>
      {{ loading ? 'Loading...' : title }}
    </h2>
  </div>
</template> 
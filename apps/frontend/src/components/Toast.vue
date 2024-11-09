<script setup lang="ts">
import type {Toast} from "@/types/toast";
import {computed, onMounted, onUnmounted, ref} from "vue";
import Icon from "@/components/icon.vue";

const props = defineProps<Toast>();
const emit = defineEmits<{
  (e: 'close', id: number): void
}>();

const progress = ref(99);
let intervalId: ReturnType<typeof setInterval>;

onMounted(() => {
  if (!props.sticky) {
    intervalId = setInterval(() => {
      progress.value -= 0.5;
    }, props.duration / 200);
  }
})

onUnmounted(() => {
  clearInterval(intervalId);
  emit('close', props.id);
});

const borderClass = computed(() => {
  switch (props.type) {
    case "info":
      return "border-info";
    case "success":
      return "border-success";
    case 'error':
      return 'border-error';
    case 'warning':
      return 'border-warning';
    default:
      return 'border-primary';
  }
});

const progressClass = computed(() => {
  switch (props.type) {
    case "info":
      return "bg-info";
    case "success":
      return "bg-success";
    case 'error':
      return 'bg-error';
    case 'warning':
      return 'bg-warning';
    default:
      return 'bg-primary';
  }
});

const icon = computed(() => {
  switch (props.type) {
    case "info":
      return "info";
    case "success":
      return "task_alt";
    case 'error':
      return 'error';
    case 'warning':
      return 'warning';
    default:
      return 'info';
  }
});
</script>

<template>
  <div class="relative bg-base-200 shadow-lg p-4 mb-4 border-l-4" :class="[borderClass]">
    <div class="flex items-center justify-between">
      <Icon v-if="icon" :name="icon" class="mr-2" />
      <p>{{ message }}</p>
      <button @click="emit('close', id)" class="btn btn-xs btn-square btn-ghost ml-2">×</button>
    </div>

    <div class="absolute bottom-0 left-0 right-0 h-1 bg-base-300">
      <div
          class="h-full transition-all duration-100 ease-linear"
          :class="[progressClass]"
          :style="{ width: `${progress}%` }"
      />
    </div>
  </div>
</template>

<style scoped>

</style>
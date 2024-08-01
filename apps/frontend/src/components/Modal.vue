<script setup lang="ts">
import {ref} from "vue";
import {onClickOutside} from "@vueuse/core";

const props = defineProps<{
  modelValue?: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>();

const modal = ref<HTMLElement | null>(null);

onClickOutside(modal, () => {
  emit('update:modelValue', false);
});
</script>

<template>
  <dialog :open="modelValue" class="modal backdrop-blur" >
    <div class="modal-box" ref="modal">
      <button @click="() => emit('update:modelValue', false)" class="btn btn-sm btn-square btn-ghost absolute right-2 top-2">✕</button>
      <slot />
    </div>
  </dialog>
</template>

<style scoped>

</style>
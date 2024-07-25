<script setup lang="ts">
const props = defineProps<{
  modelValue?: string;
  name: string;
  autocomplete?: string;
  placeholder: string;
  type?: string;
  error?: string;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()
</script>

<template>
  <div>
    <div class="label">
      <span v-if="error" class="label-text text-error">{{error}}</span>
    </div>
    <label class="transition-all input input-bordered flex items-center gap-2" :class="{ 'input-error': !!error }">
      <slot name="icon-prepend"></slot>
      <input :value="modelValue" @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)" :type="type ?? 'text'" class="w-full" :name :autocomplete :placeholder />
      <slot name="icon-append"></slot>
    </label>
  </div>
</template>

<style scoped>

</style>
<script setup lang="ts">
import {computed} from "vue";

const props = defineProps<{
  modelValue?: string;
  name: string;
  autocomplete?: string;
  placeholder: string;
  type?: string;
  error?: string;
  size?: string;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void,
  (e: 'update'): void,
  (e: 'cancel'): void
}>()

const inputSizeClass = computed(() => {
  if (props.size == 'sm') {
    return 'input-sm';
  } else if (props.size == 'lg') {
    return 'input-lg';
  } else {
    return '';
  }
})

const btnSizeClass = computed(() => {
  if (props.size == 'sm') {
    return 'btn-sm';
  } else if (props.size == 'lg') {
    return 'btn-lg';
  } else {
    return '';
  }
})
</script>

<template>
  <div class="form-control w-full">
    <div class="label">
      <span v-if="error" class="label-text text-error">{{error}}</span>
    </div>
    <div class="join">
      <input class="input input-bordered w-full join-item input-sm"
             :class="{ 'input-error': !!error, [inputSizeClass]: !!size }"
             :value="modelValue"
             @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
             :type="type ?? 'text'"
             :name
             :autocomplete
             :placeholder
      />
      <button @click="emit('update')" class="btn btn-primary join-item btn-sm" :class="[btnSizeClass]">Submit</button>
      <button @click="emit('cancel')" class="btn btn-secondary join-item btn-sm" :class="[btnSizeClass]">Cancel</button>
    </div>
  </div>
</template>

<style scoped>

</style>
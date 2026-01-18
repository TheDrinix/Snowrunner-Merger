<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';

const props = withDefaults(defineProps<{
  options: { label: string; value: string }[];
  modelValue: string[];
  placeholder?: string;
  label?: string;
}>(), {
  placeholder: 'Select options...',
  label: ''
})

// Emits
const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void
}>();

// State
const isOpen = ref(false);
const searchQuery = ref('');
const containerRef = ref<HTMLBaseElement | null>(null);

// Computed: Filter options based on search
const filteredOptions = computed(() => {
  if (!searchQuery.value) return props.options;
  return props.options.filter((option) =>
      option.label.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

// Computed: Get full objects of selected items for display
const selectedOptions = computed(() => {
  return props.options.filter((option) =>
      props.modelValue.includes(option.value)
  );
});

// Methods
const toggleDropdown = () => {
  isOpen.value = !isOpen.value;
  if (isOpen.value) {
    // Focus search input on open (optional logic can go here)
  }
};

const isSelected = (value: string) => {
  return props.modelValue.includes(value);
};

const toggleOption = (value: string) => {
  const newValue = [...props.modelValue];
  const index = newValue.indexOf(value);
  
  console.log('Toggle option:', value, 'Index:', index);

  if (index === -1) {
    newValue.push(value); // Add
  } else {
    newValue.splice(index, 1); // Remove
  }
  emit('update:modelValue', newValue);
};

const removeTag = (value: string) => {
  const newValue = props.modelValue.filter((v) => v !== value);
  emit('update:modelValue', newValue);
};

// Close dropdown when clicking outside
const handleClickOutside = (event: any) => {
  if (containerRef.value && !containerRef.value.contains(event.target)) {
    isOpen.value = false;
  }
};

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});
</script>

<template>
  <div class="w-full" ref="containerRef">
    <label v-if="label" class="label">
      <span class="label-text font-bold">{{ label }}</span>
    </label>

    <div class="relative">
      <div
          class="input input-bordered min-h-[3rem] h-auto flex flex-wrap gap-2 items-center py-2 cursor-pointer pr-10"
          @click="toggleDropdown"
      >
        <span v-if="selectedOptions.length === 0" class="text-gray-400">
          {{ placeholder }}
        </span>

        <div
            v-for="option in selectedOptions"
            :key="option.value"
            class="badge badge-primary gap-1"
        >
          {{ option.label }}
          <svg
              @click.stop="removeTag(option.value)"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              class="inline-block w-4 h-4 stroke-current cursor-pointer hover:text-white/80"
          >
            <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
            ></path>
          </svg>
        </div>

        <div class="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
          <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              class="w-5 h-5 text-gray-400 transition-transform"
              :class="{ 'rotate-180': isOpen }"
          >
            <path
                fill-rule="evenodd"
                d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z"
                clip-rule="evenodd"
            />
          </svg>
        </div>
      </div>

      <transition
          enter-active-class="transition ease-out duration-200"
          enter-from-class="opacity-0 translate-y-1"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition ease-in duration-150"
          leave-from-class="opacity-100 translate-y-0"
          leave-to-class="opacity-0 translate-y-1"
      >
        <div
            v-if="isOpen"
            class="absolute z-[9999] w-full mt-1 bg-base-100 border border-base-300 rounded-box shadow-lg max-h-60 overflow-hidden flex flex-col"
        >
          <div class="p-2 border-b border-base-200 sticky top-0 bg-base-100 z-10">
            <input
                v-model="searchQuery"
                type="text"
                class="input input-sm input-bordered w-full"
                placeholder="Search..."
            />
          </div>

          <ul class="menu w-full overflow-y-auto p-0 flex-nowrap">
            <li v-for="option in filteredOptions" :key="option.value">
              <label class="label cursor-pointer justify-start gap-3 py-3 hover:bg-base-200">
                <input
                    type="checkbox"
                    class="checkbox checkbox-primary checkbox-sm"
                    :checked="isSelected(option.value)"
                    @change="toggleOption(option.value)"
                />
                <span class="label-text flex-1">{{ option.label }}</span>
              </label>
            </li>

            <li v-if="filteredOptions.length === 0" class="text-center py-4 text-gray-500">
              No options found.
            </li>
          </ul>
        </div>
      </transition>
    </div>
  </div>
</template>
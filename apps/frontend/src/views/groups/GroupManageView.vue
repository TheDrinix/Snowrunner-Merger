<script setup lang="ts">
import {useRoute} from "vue-router";
import {computed} from "vue";

const route = useRoute();

const groupId = computed(() => {
  return parseInt(route.params.id as string);
});
</script>

<template>
{{groupId}}
</template>

<style scoped>

</style>
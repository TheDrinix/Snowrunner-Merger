<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed} from "vue";

const route = useRoute();
const router = useRouter();

if (route.name == 'auth') {
  router.push({name: 'login'});
}

const title = computed(() => {
  return route.name == 'login' ? 'Sign in to existing account' : 'Create a new account'
})
</script>

<template>
  <div class="card w-5/6 md:w-2/3 lg:w-1/2 mx-auto bg-base-200 shadow-xl">
    <div class="card-header">
      {{ title }}
    </div>
    <div class="card-body pt-4">
      <RouterView />
      <hr class="divider border-none my-2" />
      <div>
        <h3>Sign-in using external service</h3>
        <div class="flex flex-row gap-4 mt-2">
          Google sign in here
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
<script setup lang="ts">
import Modal from "@/components/Modal.vue";
import { ref } from "vue";
import { useRouter } from "vue-router";
import {useHttp} from "@/composables/useHttp";
import type {Group} from "@/types/groups";
import {useGroupsStore} from "@/stores/groupsStore";
import {useToaster} from "@/stores/toastStore";


const props = defineProps<{
  group: Group
}>()

const http = useHttp();
const router = useRouter()
const groupsStore = useGroupsStore();
const {createToast} = useToaster();

const isModalOpen = ref(false);

const handleGroupDelete = async () => {
  try {
    await http.delete(`/groups/${props.group.id}`);

    groupsStore.removeGroup(props.group.id);
    
    createToast('Group deleted', `The group ${props.group.name} has been deleted successfully.`, 'success');
  } catch (e: any) {
    if (e.response.data.title) {
      createToast(e.response.data.title, e.response.data.message || '', 'error');
    }
  }

  router.push({name: 'groups'});
}
</script>

<template>
  <div>
    <button class="btn btn-outline btn-error btn-sm w-full md:w-fit" @click="isModalOpen = true">Delete group</button>
    <Modal v-model="isModalOpen">
      <h2 class="text-lg">Are you sure you want to delete group {{group?.name}}?</h2>
      <div class="modal-action">
        <button class="btn btn-neutral" @click="isModalOpen = false">Cancel</button>
        <button class="btn btn-error" @click="handleGroupDelete">Delete</button>
      </div>
    </Modal>
  </div>
</template>

<style scoped>

</style>
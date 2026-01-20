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

const handleGroupLeave = () => {
  http.delete(`/groups/${props.group.id}/leave`)
      .then(res => {
        groupsStore.removeGroup(props.group.id);
        
        createToast('Left group', `You have left the group ${props.group.name}.`, 'success');
      })
      .catch(e => {
        if (e.response.data.title) {
          createToast(e.response.data.title, '', 'error');
        }
      });

  router.push({name: 'groups'});
}
</script>

<template>
  <div>
    <button class="btn btn-outline btn-error btn-sm" @click="isModalOpen = true">Leave</button>
    <Modal v-model="isModalOpen">
      <h2 class="text-lg">Are you sure you want to leave {{group?.name}}?</h2>
      <div class="modal-action">
        <button class="btn btn-neutral" @click="isModalOpen = false">Cancel</button>
        <button class="btn btn-error" @click="handleGroupLeave">Leave</button>
      </div>
    </Modal>
  </div>
</template>

<style scoped>

</style>
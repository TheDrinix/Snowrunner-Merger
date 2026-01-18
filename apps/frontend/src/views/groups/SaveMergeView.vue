<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useGroupsStore} from "@/stores/groupsStore";
import JSZip from "jszip";
import {generateSaveRegex, validateSaveFiles} from "@/helpers/saves";
import {useHttp} from "@/composables/useHttp";
import {useToaster} from "@/stores/toastStore";
import type { AxiosRequestConfig } from "axios";
import SaveMergingInstructions from "@/components/groups/SaveMergingInstructions.vue";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();
const http = useHttp();
const { createToast } = useToaster();

const groupId = computed(() => {
  return route.params.id as string;
});

const saveNumber = computed<number>(() => {
  return (route.query.saveNumber as string | undefined) ? parseInt(route.query.saveNumber as string) : 0;
});

const group = computed(() => {
  return groupsStore.getGroup(groupId.value);
});

if (!group.value) {
  router.push({name: 'groups'});
}

const loading = ref(false);

const fileInput = ref<HTMLInputElement | null>(null);

const formData = ref<{
  files?: FileList,
  saveNumber?: number,
  outputSaveNumber?: number
}>({});
const canMerge = computed(() => {
  return !!formData.value.files && formData.value.saveNumber != undefined && formData.value.outputSaveNumber != undefined;
});
const availableSaves = ref<number[]>([]);
const error = ref<string | null>(null);

const handleFolderChange = async (event: Event) => {
  const files = (event.target as HTMLInputElement).files;

  if (!files) return;

  const fileNames = [...files].map(file => file.name);

  availableSaves.value = validateSaveFiles(fileNames);

  if (availableSaves.value.length === 0) {
    error.value = "No valid saves found in the selected folder";

    if (fileInput.value) {
      fileInput.value.value = "";
    }
    return;
  }

  formData.value.files = files;
  formData.value.saveNumber = availableSaves.value[0];
  formData.value.outputSaveNumber = 0;
}

const handleSaveMerge = async () => {
  if (!formData.value.files || formData.value.saveNumber === undefined || formData.value.outputSaveNumber === undefined) return;

  loading.value = true;

  const regex = generateSaveRegex(formData.value.saveNumber)

  const zip = new JSZip();

  for (let file of formData.value.files) {
    if (regex.test(file.name)) {
      console.log(`Passed: ${file.name}`)
      zip.file(file.name, file);
    }
  }

  const zipBlob = await zip.generateAsync({type: "blob"});

  const body = new FormData();

  body.set('saveNumber', formData.value.saveNumber.toString());
  body.set('outputSaveNumber', formData.value.outputSaveNumber.toString());
  body.set('save', zipBlob);

  const cfg: AxiosRequestConfig = {
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    params: {
      storedSaveNumber: saveNumber.value
    },
    responseType: 'blob'
  };

  try {
    const res = await http.post(`/groups/${groupId.value}/merge`, body, cfg);

    const url = window.URL.createObjectURL(res.data);

    const a = document.createElement('a');
    a.href = url;
    a.setAttribute('download', `merged_save.zip`);
    document.body.appendChild(a);
    a.click();

    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
    
    router.push({
      name: 'group-save-merge-success', 
      params: {
        id: groupId.value
      }
    });
  } catch (e: any) {
    if (e.response.data.title) {

      createToast(e.response.data.title, '', 'error', 10000);
    }
  }

  loading.value = false;
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:w-full bg-base-200 shadow-xl border border-base-300">
    <div class="card-body">
      <h2 class="card-title text-2xl font-bold mb-6 border-b border-base-300 pb-2">Merge save</h2>

      <div class="flex flex-col-reverse lg:flex-row gap-8">
        <div class="w-full lg:w-3/5">
          <form @submit.prevent="handleSaveMerge">
            <div class="flex flex-col gap-4">
              <div class="alert alert-error" v-if="error">
                <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                <span>{{error}}</span>
              </div>

              <div class="form-control w-full">
                <div class="label pt-0">
                  <span class="label-text font-semibold">1. Select a snowrunner save folder</span>
                </div>
                <input class="file-input file-input-bordered file-input-primary w-full" ref="fileInput" type="file" webkitdirectory directory @change="handleFolderChange" />
              </div>

              <div class="transition-all duration-500 ease-in-out overflow-hidden" :class="{'max-h-64 opacity-100': availableSaves.length, 'max-h-0 opacity-0': !availableSaves.length}">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="form-control w-full">
                    <div class="label">
                      <span class="label-text font-semibold">2. Save to merge (Source)</span>
                    </div>
                    <select name="saveNumber" class="select select-bordered focus:select-primary" v-model="formData.saveNumber" :disabled="!availableSaves.length">
                      <option v-for="save in availableSaves" :key="save" :value="save">Save {{save + 1}}</option>
                    </select>
                  </div>

                  <div class="form-control w-full">
                    <div class="label">
                      <span class="label-text font-semibold">3. Output slot (Target)</span>
                    </div>
                    <select name="outputSaveNumber" class="select select-bordered focus:select-primary" v-model="formData.outputSaveNumber" :disabled="!availableSaves.length">
                      <option v-for="i in 4" :key="i - 1" :value="i - 1">Save slot {{i}}</option>
                    </select>
                  </div>
                </div>
              </div>

              <div class="pt-4">
                <button :disabled="!canMerge || loading" type="submit" class="btn btn-primary btn-block lg:btn-wide transition-all shadow-lg">
                  <span v-if="loading" class="loading loading-dots"></span>
                  <span v-else>Merge Saves</span>
                </button>
              </div>
            </div>
          </form>
        </div>

        <div class="divider lg:divider-horizontal mx-0"></div>

        <SaveMergingInstructions />

      </div>
    </div>
  </div>
</template>

<style scoped>
</style>
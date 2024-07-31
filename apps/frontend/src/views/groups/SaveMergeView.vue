<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useGroupsStore} from "@/stores/groupsStore";
import JSZip from "jszip";
import {validateSaveFiles} from "@/helpers/saves";
import {useHttp} from "@/composables/useHttp";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();
const http = useHttp();

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
  if (!formData.value.files || !formData.value.saveNumber || !formData.value.outputSaveNumber) return;

  const prefix = formData.value.saveNumber ? formData.value.saveNumber + '_' : '';

  const regexString = `(CompleteSave${formData.value.saveNumber}\\.dat)|(\\b${prefix}(fog|sts)_.*\\.dat\\b)`;

  const regex = new RegExp(regexString, 'm');

  const zip = new JSZip();

  for (let file of formData.value.files) {
    if (regex.test(file.name)) {
      console.log(`Passed: ${file.name}`);
      zip.file(file.name, file);
    }
  }

  const zipBlob = await zip.generateAsync({type: "blob"});

  const body = new FormData();

  body.set('saveNumber', formData.value.saveNumber.toString());
  body.set('outputSaveNumber', formData.value.outputSaveNumber.toString());
  body.set('save', zipBlob);

  const res = await http.post(`/groups/${groupId.value}/merge`, body, {
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    params: {
      storedSaveNumber: saveNumber.value
    },
    responseType: 'blob'
  });

  if (res.status < 300) {
    const url = window.URL.createObjectURL(res.data);

    const a = document.createElement('a');
    a.href = url;
    a.setAttribute('download', `merged_save.zip`);
    document.body.appendChild(a);
    a.click();

    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:w-full bg-base-200 shadow-xl">
    <div class="card-header">
      <h2 class="card-title">Merge save</h2>
    </div>
    <div class="card-body flex-col-reverse lg:gap-4 lg:flex-row">
      <div class="w-full lg:w-1/2 xl:w-3/5">
        <form @submit.prevent="handleSaveMerge">
          <div class="flex flex-col gap-4">
            <div class="alert alert-error" v-if="error">
              {{error}}
            </div>
            <div class="form-control w-full">
              <div class="label">
                <span class="label-text">Select a snowrunner save folder</span>
              </div>
              <input class="file-input file-input-bordered w-full" ref="fileInput" type="file" webkitdirectory directory @change="handleFolderChange" />
            </div>
            <div class="form-control w-full" v-if="availableSaves.length">
              <div class="label">
                <span class="label-text">Select which save you want to merge</span>
              </div>
              <select name="saveNumber" class="select select-bordered" v-model="formData.saveNumber">
                <option v-for="save in availableSaves" :key="save" :value="save">Save {{save + 1}}</option>
              </select>
            </div>
            <div class="form-control w-full" v-if="availableSaves.length">
              <div class="label">
                <span class="label-text">Select output save number</span>
              </div>
              <select name="outputSaveNumber" class="select select-bordered" v-model="formData.outputSaveNumber">
                <option v-for="i in 4" :key="i - 1" :value="i - 1">Save slot {{i}}</option>
              </select>
            </div>
            <div class="flex justify-center">
              <button :disabled="!canMerge" type="submit" class="btn btn-primary btn-wide transition-all">
                Merge
              </button>
            </div>
          </div>
        </form>
      </div>
      <div class="w-full lg:w-1/2 xl:w-2/5">
        <h3 class="text-lg font-medium">Instructions</h3>
        <ol class="list-decimal ml-6">
          <li class="my-1"><p>Select your save by going to:</p><p>Documents -> My games -> Snowrunner -> base -> storage</p><p>There you'll see a folder named using bunch of random alphanumeric character, that's your save folder</p></li>
          <li class="my-1">After selecting the folder, select save number which you want to merge</li>
          <li class="my-1">Select the output save number</li>
          <li class="my-1">Click merge</li>
        </ol>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
<script setup lang="ts">
import {useRoute, useRouter} from "vue-router";
import {computed, ref} from "vue";
import {useGroupsStore} from "@/stores/groupsStore";
import {generateSaveRegex, validateSaveFiles} from "@/helpers/saves";
import JSZip from "jszip";
import {useHttp} from "@/composables/useHttp";
import {useToaster} from "@/stores/toastStore";

const route = useRoute();
const router = useRouter();
const groupsStore = useGroupsStore();
const http = useHttp()
const {createToast} = useToaster();

const groupId = computed(() => {
  return route.params.id as string;
})

const saveSlot = computed<number | undefined>(() => {
  const saveNumberStr = route.query.saveNumber as string | undefined;

  return saveNumberStr ? parseInt(saveNumberStr) : undefined;
})

const group = computed(() => {
  return groupsStore.getGroup(groupId.value);
})

if (!group.value) {
  router.push({name: 'groups'});
}

const loading = ref(false);
const fileInput = ref<HTMLInputElement | null>(null);

const formData = ref<{
  files?: FileList,
  saveNumber?: number,
  description?: string
}>({});

const canUpload = computed(() => {
  return !!formData.value.files && formData.value.saveNumber != undefined && formData.value.description != undefined;
});
const availableSaves = ref<number[]>([]);
const error = ref<string | null>(null);

const handleFolderChange = async (event: Event) => {
  const files = (event.target as HTMLInputElement).files

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
}

const handleSaveUpload = async () => {
  if (!formData.value.files || formData.value.saveNumber === undefined || !formData.value.description) return;

  loading.value = true;

  const regex = generateSaveRegex(formData.value.saveNumber);

  const zip = new JSZip();

  for (let file of formData.value.files) {
    if (regex.test(file.name)) {
      zip.file(file.name, file);
    }
  }

  const zipBlob = await zip.generateAsync({type: "blob"});

  const body = new FormData()

  body.append("save", zipBlob, "saves.zip");
  body.append("description", formData.value.description);
  body.append("saveNumber", formData.value.saveNumber.toString());

  const params = new URLSearchParams();

  if (saveSlot.value !== undefined) {
    params.append("saveSlot", saveSlot.value.toString());
  }

  try {
    await http.post(`/groups/${groupId.value}/upload`, body, {
      headers: {
        "Content-Type": "multipart/form-data"
      },
      params,
    });

    groupsStore.clearSaves(groupId.value);

    await router.push({name: "group-manage", params: {id: groupId.value}});
  } catch (e: any) {
    if (e.response.data.title) {
      createToast(e.response.data.title, 'error', 10000);
    }
  }

  loading.value = false;
}
</script>

<template>
  <div class="card mx-auto w-11/12 md:w-5/6 lg:w-full bg-base-200 shadow-xl">
    <div class="card-header">
      <h2 class="card-title">Merge save</h2>
    </div>
    <div class="card-body flex-col-reverse lg:gap-4 lg:flex-row">
      <div class="w-full lg:w-1/2 xl:w-3/5">
        <form @submit.prevent="handleSaveUpload">
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
                <span class="label-text">Enter a save description</span>
              </div>
              <textarea class="textarea textarea-bordered w-full min-h-8 resize-none" v-model="formData.description" />
            </div>
            <div class="flex justify-center">
              <button :disabled="!canUpload || loading" type="submit" class="btn btn-primary btn-wide transition-all">
                Upload <span v-if="loading" class="loading loading-dots" />
              </button>
            </div>
          </div>
        </form>
      </div>
      <div class="w-full lg:w-1/2 xl:w-2/5">
        <h3 class="text-lg font-medium">Instructions</h3>
        <ol class="list-decimal ml-6">
          <li class="my-1"><p>Select your save by going to:</p><p>Documents -> My games -> Snowrunner -> base -> storage</p><p>There you'll see a folder named using bunch of random alphanumeric character, that's your save folder</p></li>
          <li class="my-1">After selecting the folder, select save number which you want to upload</li>
          <li class="my-1">Select the output save number</li>
          <li class="my-1">Click upload</li>
        </ol>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>
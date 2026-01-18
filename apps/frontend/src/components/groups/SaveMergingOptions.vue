<script setup lang="ts">
import {SaveMergingOptions} from "@/helpers/saves";
import MultiSelect from "@/components/forms/MultiSelect.vue";
import {computed} from "vue";
import type {DiscoveredMap, SaveMergingConfig} from "@/types/saves";

const props = defineProps<{
  modelValue: SaveMergingConfig;
  availableMaps: DiscoveredMap[];
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: SaveMergingConfig): void
}>();

const maps = computed(() => {
  return props.availableMaps.map(map => ({
    label: map.name,
    value: map.id
  }));
});

const createOption = (option: SaveMergingOptions) => {
  return computed({
    get() {
      return (props.modelValue.options & option) !== 0;
    },
    set(value: boolean) {
      let newOptions = props.modelValue.options;
      if (value) {
        newOptions |= option;
      } else {
        newOptions &= ~option;
      }
      emit('update:modelValue', { ...props.modelValue, options: newOptions });
    }
  })
}

const missionProgressOpt = createOption(SaveMergingOptions.MissionProgress);
const contestTimesOpt = createOption(SaveMergingOptions.ContestTimes);
const mapProgressOpt = createOption(SaveMergingOptions.MapProgress);
const discoveredVehiclesOpt = createOption(SaveMergingOptions.DiscoveredVehicles);
const unlockedUpgradesOpt = createOption(SaveMergingOptions.UnlockedUpgrades);
const worldVehiclesOpt = createOption(SaveMergingOptions.VehiclesInWorld);

const handleMapsChange = (value: string[]) => {
  emit('update:modelValue', { ...props.modelValue, maps: value });
};
</script>

<template>
    <div class="w-full">
      <div class="label">
        <span class="label-text font-semibold">4. Merging options</span>
      </div>
      <div>
        <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 auto-rows-auto">
          <div class="flex flex-row items-center form-control">
            <input v-model="missionProgressOpt" type="checkbox" id="mission-progress" name="mission-progress" class="checkbox checkbox-primary">
            <label for="mission-progress" class="label cursor-pointer">
              <span class="label-text">Mission Progress</span>
            </label>
          </div>

          <div class="flex flex-row items-center form-control">
            <input v-model="contestTimesOpt" type="checkbox" id="contest-times" name="contest-times" class="checkbox checkbox-primary">
            <label for="contest-times" class="label cursor-pointer">
              <span class="label-text">Contests times</span>
            </label>
          </div>

          <div class="flex flex-row items-center form-control">
            <input v-model="mapProgressOpt" type="checkbox" id="map-progress" name="map-progress" class="checkbox checkbox-primary">
            <label for="map-progress" class="label cursor-pointer">
              <span class="label-text">Map Progress</span>
            </label>
          </div>

          <div class="flex flex-row items-center form-control">
            <input v-model="discoveredVehiclesOpt" type="checkbox" id="discovered-vehicles" name="discovered-vehicles" class="checkbox checkbox-primary">
            <label for="discovered-vehicles" class="label cursor-pointer">
              <span class="label-text">Discovered Vehicles</span>
            </label>
          </div>

          <div class="flex flex-row items-center form-control">
            <input v-model="unlockedUpgradesOpt" type="checkbox" id="unlocked-upgrades" name="unlocked-upgrades" class="checkbox checkbox-primary">
            <label for="unlocked-upgrades" class="label cursor-pointer">
              <span class="label-text">Unlocked Upgrades</span>
            </label>
          </div>

          <div class="flex flex-row items-center form-control">
            <input v-model="worldVehiclesOpt" type="checkbox" id="world-vehicles" name="world-vehicles" class="checkbox checkbox-primary">
            <label for="world-vehicles" class="label cursor-pointer">
              <span class="label-text">Vehicles In World</span>
            </label>
          </div>

          <div class="flex form-control md:col-span-2 xl:col-span-3 w-full">
            <MultiSelect
                :options="maps"
                :model-value="modelValue.maps"
                @update:modelValue="handleMapsChange"
                label="Maps to merge (optional)"
                placeholder="Select maps if you want to limit merging to specific maps"
            />
          </div>
        </div>
      </div>
    </div>
</template>

<style scoped>

</style>
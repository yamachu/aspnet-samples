<script setup lang='ts'>
import { getCurrentInstance } from 'vue';
import { useBlazor } from './blazor-vue';

interface Props {
  title: string;
  incrementAmount: number | undefined;
  customObject: object;
  customCallback: (_: object) => void;
}

const props = defineProps<Props>();
const instance = getCurrentInstance();

const Counter = useBlazor(instance, 'counter', props)

</script>

<template>
    <Counter />
</template>

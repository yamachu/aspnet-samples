<script setup lang="ts">
// This starter template is using Vue 3 <script setup> SFCs
// Check out https://v3.vuejs.org/api/sfc-script-setup.html#sfc-script-setup
import { ref, unref } from "vue";
import Counter from "./Counter.vue"

const nextCounterIndex = ref(1);
const blazorCounters = ref<{ title: string, incrementAmount: number, customObject: any }[]>([]);
const addBlazorCounter = () => {
  const index = unref(nextCounterIndex);
  nextCounterIndex.value += 1;
  blazorCounters.value = blazorCounters.value.concat([{
    title: `Counter ${index}`,
    incrementAmount: index,
    customObject: { StringValue: 'Hello!', IntegerValue: 42 },
  }])
};
const removeBlazorCounter = () => {
  blazorCounters.value = blazorCounters.value.slice(0, -1);
};
const modifyParameters = () => {
  blazorCounters.value = blazorCounters.value.map((counter) => {
    return {
      ...counter,
      incrementAmount: counter.incrementAmount + 1,
      customObject: {
        StringValue: counter.customObject.StringValue + '!',
        IntegerValue: counter.customObject.IntegerValue - 1,
      }
    };
  });
};
const logEventArgs = (eventArgs: any) => {
  console.log(eventArgs);
};

</script>

<template>
  <div>
    <header>
      <img src="./assets/logo.png" alt="logo" />
      <p>
        <button v-on:click="addBlazorCounter">Add Blazor counter</button> &nbsp;
        <button v-on:click="removeBlazorCounter">Remove Blazor counter</button> &nbsp;
        <button v-on:click="modifyParameters">Modify parameters from JS</button>
      </p>

      
      <div v-for="counter in blazorCounters" :key="counter.title">
        <Counter
          v-bind:title="counter.title"
          v-bind:increment-amount="counter.incrementAmount"
          v-bind:custom-object="counter.customObject"
          v-bind:custom-callback="logEventArgs" >
        </Counter>
      </div>

    </header>
  </div>
</template>

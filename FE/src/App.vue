<template>
  <div id="app">
    <router-view />
  </div>
</template>
<script >
import { loadStyles } from "@/loadStyles";


export default {
  setup() {
    loadStyles()
    return {};
  },
  name: "App",
  data() {
    return {
    };
  },
};
</script>

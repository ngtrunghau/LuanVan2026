<template>
    <div class="row">
      <div class="col-sm-7 col-auto">
        <h3 class="page-title">{{ title }}</h3>
      </div>
  </div>
</template>

<script setup>
import { computed } from "vue";
const props = defineProps({
  title: {
    type: String,
    default: ""
  }
});
</script>

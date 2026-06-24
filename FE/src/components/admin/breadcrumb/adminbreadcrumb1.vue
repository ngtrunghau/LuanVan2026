<template>
  <!-- Page Header -->
  <div class="page-header">
    <div class="row">
      <div class="col">
        <h3 class="page-title">{{ title }}</h3>
        <ul class="breadcrumb" v-if="isHomeRoute">
          <li class="breadcrumb-item active">{{ text }}</li>
        </ul>
        <ul class="breadcrumb" v-else>
          <li class="breadcrumb-item">
            <router-link to="/quan-tri">{{ text }}</router-link>
          </li>
          <li class="breadcrumb-item active">{{ text1 }}</li>
        </ul>
      </div>
    </div>
  </div>
  <!-- /Page Header -->
</template>
<script setup>
import { computed, getCurrentInstance } from "vue";
const props = defineProps({
  title: {
    type: String,
    default: ""
  },
  text: {
    type: String,
    default: ""
  },
  text1: {
    type: String,
    default: ""
  }
});
const {
  proxy
} = getCurrentInstance();
const isHomeRoute = computed(() => {
  return proxy.$route.path === "/quan-tri";
});
</script>

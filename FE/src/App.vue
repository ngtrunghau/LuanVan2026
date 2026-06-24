<template>
  <div id="app">
    <router-view />
    <AssistantChatbox v-if="showAssistantChatbox" />
  </div>
</template>
<script setup>
import { computed, getCurrentInstance, reactive, toRefs } from "vue";
import { loadStyles } from "@/loadStyles";
import AssistantChatbox from "@/components/frontend/AssistantChatbox.vue";
defineOptions({
  name: "App"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({});
loadStyles();
const showAssistantChatbox = computed(() => {
  const path = proxy.$route?.path || "";
  return !path.startsWith("/quan-tri");
});
</script>

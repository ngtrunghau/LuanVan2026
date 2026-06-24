<template>
    <ckeditor style="width: 1000px;" :editor="editor" v-model="value" :config="editorConfig">

    </ckeditor>
</template>

<script setup>
import ClassicEditor from "@/components/ckeditor5";
import { defineComponent, ref } from '@vue/runtime-core';
import { uploadImage } from "@/utils/functions/uploadImage";
import { watch } from "vue";
const props = defineProps({
  editorData: {
    type: String,
    requied: true
  }
});
function uploader(editor) {
  editor.plugins.get('FileRepository').createUploadAdapter = loader => {
    return uploadImage(loader);
  };
}
const value = ref(editorData);
watch(value, value => {
  //    console.log("LOG WATCH  : ", value);
});
</script>

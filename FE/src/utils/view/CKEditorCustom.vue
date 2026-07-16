<template>
    <ckeditor style="width: 1000px;" :editor="editor" v-model="value" :config="editorConfig" />
</template>

<script setup>
import ClassicEditor from "@/components/ckeditor5";
import { ref } from "vue";
import { uploadImage } from "@/utils/functions/uploadImage";
import { watch } from "vue";
const props = defineProps({
  modelValue: {
    type: String,
    default: ""
  }
});
const emit = defineEmits(["update:modelValue"]);

function uploader(editor) {
  editor.plugins.get('FileRepository').createUploadAdapter = loader => {
    return uploadImage(loader);
  };
}

const editor = ClassicEditor;
const editorConfig = {
  extraPlugins: [uploader]
};
const value = ref(props.modelValue || "");

watch(() => props.modelValue, newValue => {
  if (newValue !== value.value) {
    value.value = newValue || "";
  }
});

watch(value, newValue => {
  emit("update:modelValue", newValue);
});
</script>
